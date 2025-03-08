using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using KeeneticVpnMaster.Models;

namespace KeeneticVpnMaster.Services.Keenetic
{
    /// <summary>
    /// Сервис для взаимодействия с Keenetic API.
    /// </summary>
    public class KeeneticService : IKeeneticService
    {
        private readonly HttpClient _httpClient;
        private string? _baseUrl;

        public bool IsAuthenticated { get; private set; }

        public event Action? Authenticated;
        public event Action<int, string>? AuthenticationFailed;
        public event Action? Disconnected;

        public KeeneticService()
        {
            _httpClient = new HttpClient(new HttpClientHandler { UseCookies = true })
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        /// <summary>
        /// Выполняет аутентификацию на Keenetic-устройстве.
        /// </summary>
        public async Task AuthenticateAsync(AuthConfig authConfig, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(authConfig.IpAddress) ||
                string.IsNullOrWhiteSpace(authConfig.Username) ||
                string.IsNullOrWhiteSpace(authConfig.Password))
            {
                throw new ArgumentException("IP address, username, and password cannot be empty.");
            }

            _baseUrl = $"https://{authConfig.IpAddress}";
            IsAuthenticated = false;

            try
            {
                Console.WriteLine($"[DEBUG] Authenticating to {_baseUrl} with username {authConfig.Username}");
                // Для авторизации используем специальный URL.
                var authUrl = GetAuthUrl();
                using var response = await _httpClient.GetAsync(authUrl, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (!response.Headers.TryGetValues("X-NDM-Realm", out var realmValues) ||
                        !response.Headers.TryGetValues("X-NDM-Challenge", out var challengeValues))
                    {
                        HandleAuthenticationFailure((int)HttpStatusCode.Unauthorized, "Failed to retrieve authentication headers.");
                        return;
                    }
                    string realm = realmValues.FirstOrDefault() ?? string.Empty;
                    string challenge = challengeValues.FirstOrDefault() ?? string.Empty;
                    string md5Hash = ComputeMd5($"{authConfig.Username}:{realm}:{authConfig.Password}");
                    string sha256Hash = ComputeSha256($"{challenge}{md5Hash}");
                    var loginData = new { login = authConfig.Username, password = sha256Hash };
                    var jsonContent = CreateJsonContent(loginData);
                    using var postResponse = await _httpClient.PostAsync(authUrl, jsonContent, cancellationToken);
                    if (postResponse.IsSuccessStatusCode)
                    {
                        HandleAuthenticationSuccess();
                        return;
                    }
                    HandleAuthenticationFailure((int)postResponse.StatusCode, "Incorrect username or password.");
                    return;
                }
                else if (response.IsSuccessStatusCode)
                {
                    HandleAuthenticationSuccess();
                    return;
                }
                HandleAuthenticationFailure((int)response.StatusCode, "Unexpected authentication error.");
            }
            catch (TaskCanceledException)
            {
                HandleAuthenticationFailure((int)HttpStatusCode.RequestTimeout, "Connection timeout exceeded.");
            }
            catch (HttpRequestException ex)
            {
                HandleAuthenticationFailure((int)HttpStatusCode.ServiceUnavailable, $"Network error: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправляет GET-запрос к Keenetic API по указанному endpoint.
        /// </summary>
        public async Task<string> GetRequestAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var url = GetUrl(endpoint);
            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestException("Request timeout exceeded.");
            }
        }

        /// <summary>
        /// Отправляет POST-запрос к Keenetic API по указанному endpoint с заданными данными.
        /// </summary>
        public async Task<string> PostRequestAsync(string endpoint, object postData, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var url = GetUrl(endpoint);
            try
            {
                var jsonContent = CreateJsonContent(postData);
                using var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestException("Request timeout exceeded.");
            }
        }

        /// <summary>
        /// Отправляет DELETE-запрос к Keenetic API по указанному endpoint.
        /// </summary>
        public async Task<string> DeleteRequestAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var url = GetUrl(endpoint);
            try
            {
                using var response = await _httpClient.DeleteAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestException("Request timeout exceeded.");
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetSystem()
        {
            return await GetRequestAsync("show/system");
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            if (!IsAuthenticated)
                return;

            IsAuthenticated = false;
            _baseUrl = null;
            _httpClient.DefaultRequestHeaders.Clear();
            Disconnected?.Invoke();
            Console.WriteLine("[INFO] Disconnected from Keenetic.");
        }

        /// <inheritdoc/>
        public async Task<string> GetLogsForTenSecondsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var logsBuilder = new StringBuilder();

            var postData = new Dictionary<string, object>
            {
                ["once"] = true,
                ["max-lines"] = 100
            };
            string postResult = await PostRequestAsync("show/log", postData, cancellationToken);
            logsBuilder.AppendLine(postResult);
            Console.WriteLine(postResult);

            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(10))
            {
                string getResult = await GetRequestAsync("show/log", cancellationToken);
                if (!string.IsNullOrWhiteSpace(getResult) && !getResult.Contains("\"continued\": true"))
                {
                    logsBuilder.AppendLine(getResult);
                }
                Console.WriteLine(getResult);
                await Task.Delay(1000, cancellationToken);
            }

            string deleteResult = await DeleteRequestAsync("show/log", cancellationToken);
            logsBuilder.AppendLine(deleteResult);
            Console.WriteLine(deleteResult);

            return logsBuilder.ToString();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WireGuardShowInterface>> GetWireGuardShowInterfacesAsync()
        {
            try
            {
                string jsonResponse = await GetRequestAsync("show/interface");
                var allInterfaces = JsonSerializer.Deserialize<Dictionary<string, WireGuardShowInterface>>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (allInterfaces != null)
                {
                    return allInterfaces
                        .Where(kvp => kvp.Key.StartsWith("Wireguard", StringComparison.OrdinalIgnoreCase))
                        .Select(kvp => kvp.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading WireGuard interfaces: {ex.Message}");
            }
            return Enumerable.Empty<WireGuardShowInterface>();
        }

        /// <inheritdoc/>
        public async Task<WireGuardShowInterface> GetWireGuardShowInterfaceAsync(string interfaceName)
        {
            try
            {
                // Отправляем GET-запрос по URL: "show/interface/{interfaceName}"
                string jsonResponse = await GetRequestAsync($"show/interface/{interfaceName}");
        
                // Десериализуем полученный JSON в объект WireGuardShowInterface
                var interfaceInfo = JsonSerializer.Deserialize<WireGuardShowInterface>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
                if (interfaceInfo == null)
                {
                    throw new Exception("Не удалось десериализовать данные интерфейса.");
                }
                return interfaceInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении информации об интерфейсе {interfaceName}: {ex.Message}");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<string> SetInterfaceStateAsync(string interfaceName, bool up, CancellationToken cancellationToken = default)
        {
            // Формируем данные в виде:
            // {
            //   "interface": {
            //      "Wireguard3": {
            //         "up": false
            //      }
            //   }
            // }
            var postData = new Dictionary<string, object>
            {
                ["interface"] = new Dictionary<string, object>
                {
                    [interfaceName] = new Dictionary<string, object>
                    {
                        ["up"] = up
                    }
                }
            };

            return await PostRequestAsync("", postData, cancellationToken);
        }
        
        /// <inheritdoc/>
        public async Task<string> DeleteInterface(string interfaceName, CancellationToken cancellationToken = default)
        {
            return await DeleteRequestAsync($"interface/{interfaceName}", cancellationToken);
        }
        
        /// <inheritdoc/>
        public async Task<string> SystemConfigurationSaveAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var postData = new 
            {
                system = new 
                {
                    configuration = new 
                    {
                        save = new { }
                    }
                }
            };

            return await PostRequestAsync("", postData, cancellationToken);
        }



        #region Вспомогательные методы

        /// <summary>
        /// Проверяет, что клиент аутентифицирован, иначе выбрасывает исключение.
        /// </summary>
        private void EnsureAuthenticated()
        {
            if (!IsAuthenticated || _baseUrl == null)
                throw new InvalidOperationException("Not authenticated. Please authenticate first.");
        }

        /// <summary>
        /// Формирует URL для команд API по заданному endpoint.
        /// </summary>
        private string GetUrl(string endpoint) => $"{_baseUrl}/rci/{endpoint.TrimStart('/')}";

        /// <summary>
        /// Формирует URL для авторизации.
        /// </summary>
        private string GetAuthUrl() => $"{_baseUrl}/auth";

        /// <summary>
        /// Создает JSON-контент из объекта.
        /// </summary>
        private static StringContent CreateJsonContent(object data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Обрабатывает неудачную аутентификацию.
        /// </summary>
        private void HandleAuthenticationFailure(int errorCode, string message)
        {
            AuthenticationFailed?.Invoke(errorCode, message);
            Console.WriteLine($"[ERROR] ({errorCode}) {message}");
        }

        /// <summary>
        /// Обрабатывает успешную аутентификацию.
        /// </summary>
        private void HandleAuthenticationSuccess()
        {
            IsAuthenticated = true;
            Authenticated?.Invoke();
            Console.WriteLine("[INFO] Authentication successful.");
        }

        /// <summary>
        /// Вычисляет MD5-хеш входной строки.
        /// </summary>
        private static string ComputeMd5(string input)
        {
            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Вычисляет SHA256-хеш входной строки.
        /// </summary>
        private static string ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        #endregion
    }
}
