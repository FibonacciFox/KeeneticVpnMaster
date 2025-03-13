using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KeeneticVpnMaster.Services;
using KeeneticVpnMaster.Services.Keenetic;
using KeeneticVpnMaster.Services.Navigation;
using KeeneticVpnMaster.ViewModels;
using KeeneticVpnMaster.ViewModels.Pages;
using Splat;
using SukiUI.Dialogs;

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
                RegisterAppServices();

                // Получаем `KeeneticVpnMasterViewModel`
                var mainViewModel = Locator.Current.GetService<KeeneticVpnMasterViewModel>();

                var mainWindow = new Views.KeeneticVpnMaster
                {
                    DataContext = mainViewModel
                };
                
                Locator.CurrentMutable.RegisterLazySingleton(() => TopLevel.GetTopLevel(mainWindow), typeof(TopLevel));

                desktop.MainWindow = mainWindow;
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void RegisterAppServices()
        {
            //Регистрируем singleton для AppConfigService
            Locator.CurrentMutable.RegisterLazySingleton(() => AppConfigService.Load(), typeof(AppConfigService));

            //Регистрируем singleton для KeeneticService
            Locator.CurrentMutable.RegisterLazySingleton<IKeeneticService>(() => new KeeneticService());
            
            //Регистрируем singleton для SukiDialogManager
            Locator.CurrentMutable.RegisterLazySingleton<ISukiDialogManager >(() => new SukiDialogManager());

            //Создаем и регистрируем `KeeneticVpnMasterViewModel`
            var mainViewModel = new KeeneticVpnMasterViewModel();
            Locator.CurrentMutable.RegisterConstant(mainViewModel, typeof(KeeneticVpnMasterViewModel));
            

            //Теперь можно создать `NavigationService`, так как `mainViewModel` уже есть
            var navigationService = new NavigationService(mainViewModel);

            //Регистрируем `INavigationService`
            Locator.CurrentMutable.RegisterConstant(navigationService, typeof(INavigationService));
            
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
