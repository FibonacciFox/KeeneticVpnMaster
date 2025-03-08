using System;
using Avalonia;
using Avalonia.ReactiveUI;
using KeeneticVpnMaster.Converters;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Projektanker.Icons.Avalonia.MaterialDesign;

namespace KeeneticVpnMaster
{
    sealed class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
        {
            IconProvider.Current.Register<FontAwesomeIconProvider>().Register<MaterialDesignIconProvider>().Register<KeeneticVpnMasterIconProvider>();
            return AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace().UseReactiveUI();
        }
    }
}