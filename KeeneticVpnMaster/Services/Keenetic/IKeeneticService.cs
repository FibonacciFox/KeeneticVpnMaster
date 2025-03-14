using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KeeneticVpnMaster.Models;

namespace KeeneticVpnMaster.Services.Keenetic
{
    /// <summary>
    /// Интерфейс для взаимодействия с Keenetic API.
    /// </summary>
    public interface IKeeneticService
    {
        /// <summary>
        /// Возвращает признак того, что клиент аутентифицирован.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Событие успешной аутентификации.
        /// </summary>
        event Action? Authenticated;

        /// <summary>
        /// Событие неудачной аутентификации.
        /// </summary>
        event Action<int, string>? AuthenticationFailed;

        /// <summary>
        /// Событие отключения клиента.
        /// </summary>
        event Action? Disconnected;

        /// <summary>
        /// Выполняет аутентификацию на Keenetic-устройстве.
        /// </summary>
        /// <param name="authConfig">Конфигурация аутентификации.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task AuthenticateAsync(AuthConfig authConfig, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправляет POST-запрос к Keenetic API по указанному endpoint с заданными данными.
        /// </summary>
        Task<string> PostRequestAsync(string endpoint, object postData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправляет DELETE-запрос на Keenetic-устройство.
        /// </summary>
        /// <param name="endpoint">Конечная точка API.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Строка с ответом от сервера.</returns>
        Task<string> DeleteRequestAsync(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отключает клиента и завершает сессию.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Получает логи за 10 секунд, используя следующую логику:
        /// 1. Отправляет POST-запрос к "rci/show/log" с данными {"once": true, "max-lines": 100} для получения текущего лога.
        /// 2. В течение 10 секунд периодически отправляет GET-запросы к "rci/show/log" для получения новых записей.
        ///    Если в ответе отсутствует флаг "continued": true, новые данные добавляются к результату.
        /// 3. По окончании 10 секунд отправляет DELETE-запрос к "rci/show/log" для прекращения получения логов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Строка, содержащая накопленные логи за 10 секунд.</returns>
        Task<string> GetLogsForTenSecondsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получает информацию об интерфейсах WireGuard.
        /// </summary>
        /// <returns>Коллекция интерфейсов WireGuard.</returns>
        Task<IEnumerable<WireGuardInterfaceInfo>> GetWireGuardShowInterfacesAsync();

        /// <summary>
        /// Получает информацию об конкретном интерфейсе WireGuard.
        /// </summary>
        /// <param name="interfaceName">Имя интерфейса, например "Wireguard0".</param>
        /// <returns>Объект типа WireGuardShowInterface с данными интерфейса.</returns>
        Task<WireGuardInterfaceInfo> GetWireGuardShowInterfaceAsync(string interfaceName);
        
        /// <summary>
        /// Включает или отключает указанный интерфейс.
        /// </summary>
        /// <param name="interfaceName">Название интерфейса, например "Wireguard3".</param>
        /// <param name="up">Если true, интерфейс будет включен, если false – отключен.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Строка с ответом от сервера.</returns>
        Task<string> SetInterfaceStateAsync(string interfaceName, bool up, CancellationToken cancellationToken = default);

        /// <summary>
        /// Создает указанный интерфейс через Keenetic API.
        /// </summary>
        /// <param name="interfaceName">Название интерфейса, который необходимо создать.</param>
        /// <param name="wireGuardConfigurationInterface">Конфигурация интерфейса.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Строка с ответом от сервера после выполнения операции удаления интерфейса.</returns>
        Task<string> CreateInterface(string interfaceName, WireGuardConfigurationInterface wireGuardConfigurationInterface, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Удаляет (отключает) указанный интерфейс через Keenetic API.
        /// Отправляет DELETE-запрос на endpoint "interface/{interfaceName}" для удаления интерфейса с заданным именем.
        /// </summary>
        /// <param name="interfaceName">Название интерфейса, который необходимо удалить.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Строка с ответом от сервера после выполнения операции удаления интерфейса.</returns>
        Task<string> DeleteInterface(string interfaceName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Сохраняет конфигурацию системы, отправляя следующие данные:
        /// {
        ///   "system": {
        ///     "configuration": {
        ///       "save": {}
        ///     }
        ///   }
        /// }
        /// Используется пустой endpoint для PostRequestAsync, который обрабатывается в GetUrl.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Строка с ответом от сервера.</returns>
        Task<string> SystemConfigurationSaveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получает системную информацию.
        /// </summary>
        /// <returns>Строка с системной информацией.</returns>
        Task<string> GetSystem();
    }
}
