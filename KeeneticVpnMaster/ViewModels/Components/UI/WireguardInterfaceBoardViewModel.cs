using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using KeeneticVpnMaster.Models;
using KeeneticVpnMaster.Services.Keenetic;
using ReactiveUI;
using Splat;

namespace KeeneticVpnMaster.ViewModels.Components.UI
{
    public class WireguardInterfaceBoardViewModel : ViewModelBase
    {
        private readonly IKeeneticService? _keeneticService = Locator.Current.GetService<IKeeneticService>();
        public ObservableCollection<WireGuardShowInterface> WireGuardShowInterfaces { get; set; } = new();
        
        private WireGuardShowInterface? _selectedInterface;
        
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        
        public WireGuardShowInterface? SelectedInterface
        {
            get => _selectedInterface;
            set => this.RaiseAndSetIfChanged(ref _selectedInterface, value);
        }
        
        private DispatcherTimer _timer;
        
        public WireguardInterfaceBoardViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += async (s, e) => await LoadInterfacesAsync();
            
            _timer.Start();
            
            EditCommand = ReactiveCommand.CreateFromTask(EditInterfaceAsync);
        }
        
        private Task EditInterfaceAsync()
        {
            if (SelectedInterface != null)
            {
                Console.WriteLine($"Редактирование интерфейса: {SelectedInterface.InterfaceName}");
                // Реализация редактирования
            }

            return Task.CompletedTask;
        }
        
        private async Task LoadInterfacesAsync()
        {
            try
            {
                // Получаем коллекцию интерфейсов
                var interfaces = await _keeneticService!.GetWireGuardShowInterfacesAsync();

                // Получаем список ID интерфейсов с сервера
                var newInterfaceIds = interfaces.Select(i => i.Id).ToHashSet();

                // Удаляем интерфейсы, которых больше нет на сервере
                var interfacesToRemove = WireGuardShowInterfaces
                    .Where(existing => !newInterfaceIds.Contains(existing.Id))
                    .ToList();

                foreach (var iface in interfacesToRemove)
                {
                    WireGuardShowInterfaces.Remove(iface);
                }

                foreach (var newInterface in interfaces)
                {
                    var existingInterface = WireGuardShowInterfaces.FirstOrDefault(i => i.Id == newInterface.Id);

                    if (existingInterface == null)
                    {
                        // Перед добавлением в коллекцию назначаем команду
                        newInterface.ToggleConnectionCommand =
                            ReactiveCommand.CreateFromTask(() => ToggleConnectionAsync(newInterface));

                        // Добавляем новый интерфейс
                        WireGuardShowInterfaces.Add(newInterface);
                    }
                    else
                    {
                        // Обновляем существующий объект (UI обновится автоматически)
                        UpdateInterface(existingInterface, newInterface);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки интерфейсов: {ex.Message}");
            }

        }

        /// <summary>
        /// Обновляет данные у существующего интерфейса, в дальнейшем исправить ложное возвращение TogleButton беред сохранением
        /// </summary>
        private void UpdateInterface(WireGuardShowInterface existingInterface, WireGuardShowInterface newData)
        {
            existingInterface.InterfaceName = newData.InterfaceName;
            existingInterface.Description = newData.Description;
            existingInterface.Type = newData.Type;
            existingInterface.Link = newData.Link;
            existingInterface.Connected = newData.Connected;
            
            existingInterface.State = newData.State;
            
            existingInterface.Mtu = newData.Mtu;
            existingInterface.Address = newData.Address;
            existingInterface.Mask = newData.Mask;
            existingInterface.Uptime = newData.Uptime;
            existingInterface.WireGuard = newData.WireGuard;
        }

        /// <summary>
        /// Переключает состояние интерфейса (включить/отключить VPN)
        /// </summary>
        private async Task ToggleConnectionAsync(WireGuardShowInterface wireGuardInterface)
        {
            _timer.Stop();

            if (_keeneticService == null) return;
            
            try
            {
                switch (wireGuardInterface.State)
                {
                    case "up":
                        await _keeneticService.SetInterfaceStateAsync(wireGuardInterface.Id, true);
                        await _keeneticService.SystemConfigurationSaveAsync();
                        break;
                    case "down":
                        await _keeneticService.SetInterfaceStateAsync(wireGuardInterface.Id, false);
                        await _keeneticService.SystemConfigurationSaveAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при переключении подключения: {ex.Message}");
            }
            finally
            {
                _timer.Start();
            }
        }
    }
}
