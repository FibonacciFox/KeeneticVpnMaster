using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using KeeneticVpnMaster.Services;
using KeeneticVpnMaster.Services.Keenetic;
using KeeneticVpnMaster.Services.Navigation;
using ReactiveUI;


namespace KeeneticVpnMaster.ViewModels.Pages
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IKeeneticService _keeneticService;
        private readonly AppConfigService _configService;
        private readonly INavigationService _navigationService;

        private string _ipAddressKeenetic = null!;
        public string IpAddressKeenetic
        {
            get => _ipAddressKeenetic;
            set => this.RaiseAndSetIfChanged(ref _ipAddressKeenetic, value);
        }

        private string _username = null!;
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string _password = null!;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set => this.RaiseAndSetIfChanged(ref _rememberMe, value);
        }

        private string _authenticationInfo = null!;
        public string AuthenticationInfo
        {
            get => _authenticationInfo;
            private set => this.RaiseAndSetIfChanged(ref _authenticationInfo, value);
        }

        private bool _isAuthenticationInfoVisible;
        public bool IsAuthenticationInfoVisible
        {
            get => _isAuthenticationInfoVisible;
            private set => this.RaiseAndSetIfChanged(ref _isAuthenticationInfoVisible, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> ForgotPasswordCommand { get; }

        public LoginPageViewModel(IKeeneticService keeneticService, AppConfigService configService, INavigationService navigationService)
        {
            _keeneticService = keeneticService ?? throw new ArgumentNullException(nameof(keeneticService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            // Загружаем настройки
            LoadConfig();

            LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
            ForgotPasswordCommand = ReactiveCommand.Create(ForgotPassword);
            
            _keeneticService.AuthenticationFailed += KeeneticApiOnAuthenticationFailed;
            _keeneticService.Authenticated += KeeneticServiceOnAuthenticated;
            
        }

        private void KeeneticServiceOnAuthenticated()
        {
            _navigationService.NavigateTo<MainPageViewModel>();
        }

        private void KeeneticApiOnAuthenticationFailed(int arg1, string arg2)
        {
            IsAuthenticationInfoVisible = true;
            AuthenticationInfo = arg2;
        }
        
        private void LoadConfig()
        {
            IpAddressKeenetic = _configService.Auth.IpAddress;
            Username = _configService.Auth.Username;
            Password = _configService.Auth.Password;
            RememberMe = _configService.Auth.SaveAuth;
        }

        private async Task LoginAsync()
        {
            IsAuthenticationInfoVisible = false;
            AuthenticationInfo = string.Empty;
            IsLoading = true; // Включаем индикатор загрузки

            try
            {
                // Обновляем данные перед отправкой
                _configService.Auth.IpAddress = IpAddressKeenetic;
                _configService.Auth.Username = Username;
                _configService.Auth.Password = Password;
                _configService.Auth.SaveAuth = RememberMe;

                await _keeneticService.AuthenticateAsync(_configService.Auth);

                if (RememberMe)
                {
                    _configService.Save(); // Сохраняем конфиг
                }
                
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    AuthenticationInfo = ex.Message;
                    IsAuthenticationInfoVisible = true;
                });
            }
            finally
            {
                IsLoading = false; // Выключаем индикатор загрузки
            }
        }

        private void ForgotPassword()
        {
            AuthenticationInfo = "Функция восстановления пароля в разработке.";
            IsAuthenticationInfoVisible = true;
        }
    }
}
