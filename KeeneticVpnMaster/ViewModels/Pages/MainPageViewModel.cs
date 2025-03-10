using System.Collections.ObjectModel;
using KeeneticVpnMaster.Models;
using KeeneticVpnMaster.Services.Keenetic;

namespace KeeneticVpnMaster.ViewModels.Pages;

public class MainPageViewModel : ViewModelBase
{
    private readonly IKeeneticService _keeneticService;

    public ObservableCollection<WireGuardInterfaceInfo> WireGuardInterfaces { get; set; } = new ObservableCollection<WireGuardInterfaceInfo>();

    public MainPageViewModel(IKeeneticService keeneticService)
    {
        _keeneticService = keeneticService;
        // Загружаем интерфейсы асинхронно
        LoadInterfacesAsync();
    }

    private async void LoadInterfacesAsync()
    {
        try
        {
            // Получаем коллекцию интерфейсов
            var interfaces = await _keeneticService.GetWireGuardShowInterfacesAsync();
            // Очищаем и заполняем коллекцию (если требуется)
            WireGuardInterfaces.Clear();
            foreach (var iface in interfaces)
            {
                WireGuardInterfaces.Add(iface);
            }
        }
        catch (System.Exception ex)
        {
            // Обработка ошибок
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки интерфейсов: {ex.Message}");
        }
    }
    
}