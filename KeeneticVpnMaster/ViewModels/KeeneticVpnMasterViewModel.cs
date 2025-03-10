using Avalonia.Styling;
using KeeneticVpnMaster.Services;
using ReactiveUI;
using Splat;
using SukiUI;
using SukiUI.Dialogs;

namespace KeeneticVpnMaster.ViewModels
{
    public class KeeneticVpnMasterViewModel : ViewModelBase
    {
        public ISukiDialogManager? DialogManager { get; } = Locator.Current.GetService<ISukiDialogManager>();
        
        private object? _content;
        public object? Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDarkTheme, value);
                ApplyTheme();
            }
        }

        public KeeneticVpnMasterViewModel()
        {
            LoadThemeFromConfig();
        }

        private void ApplyTheme()
        {
            var themeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            SukiTheme.GetInstance().ChangeBaseTheme(themeVariant);
            SaveThemeToConfig(themeVariant);
        }

        private void LoadThemeFromConfig()
        {
            IsDarkTheme = Locator.Current.GetService<AppConfigService>()!.UserPreferences.ThemeVariant == "Dark";
        }

        private void SaveThemeToConfig(ThemeVariant themeVariant)
        {
            var config = Locator.Current.GetService<AppConfigService>();
            config.UserPreferences.ThemeVariant = themeVariant.ToString();
            config.Save();
        }
    }
}