using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KeeneticVpnMaster.Services;
using KeeneticVpnMaster.Services.Keenetic;
using KeeneticVpnMaster.Services.Navigation;
using KeeneticVpnMaster.ViewModels;
using KeeneticVpnMaster.ViewModels.Components.UI;
using KeeneticVpnMaster.ViewModels.Pages;
using Splat;

namespace KeeneticVpnMaster
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Регистрируем сервисы в правильном порядке
                RegisterServices();

                // Получаем `KeeneticVpnMasterViewModel` (он уже создан корректно)
                var mainViewModel = Locator.Current.GetService<KeeneticVpnMasterViewModel>();

                var mainWindow = new Views.KeeneticVpnMaster
                {
                    DataContext = mainViewModel
                };

                desktop.MainWindow = mainWindow;
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void RegisterServices()
        {
            //Регистрируем singleton для AppConfigService
            Locator.CurrentMutable.RegisterLazySingleton(() => AppConfigService.Load(), typeof(AppConfigService));

            //Регистрируем singleton для KeeneticService
            Locator.CurrentMutable.RegisterLazySingleton<IKeeneticService>(() => new KeeneticService());

            //Создаем `KeeneticVpnMasterViewModel`
            var mainViewModel = new KeeneticVpnMasterViewModel();

            //Регистрируем его в `Splat`
            Locator.CurrentMutable.RegisterConstant(mainViewModel, typeof(KeeneticVpnMasterViewModel));

            //Теперь можно создать `NavigationService`, так как `mainViewModel` уже есть
            var navigationService = new NavigationService(mainViewModel);

            //Регистрируем `INavigationService`
            Locator.CurrentMutable.RegisterConstant(navigationService, typeof(INavigationService));

            //Теперь `LoginPageViewModel` получает `INavigationService` корректно
            Locator.CurrentMutable.Register(() => new LoginPageViewModel(
                Locator.Current.GetService<IKeeneticService>()!,
                Locator.Current.GetService<AppConfigService>()!,
                Locator.Current.GetService<INavigationService>()!
            ), typeof(LoginPageViewModel));
            
            Locator.CurrentMutable.Register(() => new MainPageViewModel(
                Locator.Current.GetService<IKeeneticService>()!
            ), typeof(MainPageViewModel));
            
            //Загружаем LoginPage при старте
            navigationService.NavigateTo<LoginPageViewModel>();
        }
    }
}
