using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public WireGuardShowInterface? SelectedInterface
        {
            get => _selectedInterface;
            set => this.RaiseAndSetIfChanged(ref _selectedInterface, value);
        }

        public ICommand EditCommand { get; }

        private readonly DispatcherTimer _timer;

        public WireguardInterfaceBoardViewModel()
        {
            // Инициализируем таймер для периодической загрузки интерфейсов
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += async (s, e) => await LoadInterfacesAsync();
            _timer.Start();

            EditCommand = ReactiveCommand.Create(() =>
            {
                // Code here will be executed when the button is clicked.
            });

            this.PropertyChanged += (sender, args) =>
            {
                Console.WriteLine(SelectedInterface.InterfaceName);
            };
        }

        private Task EditInterfaceAsync()
        {
            if (SelectedInterface != null)
            {
                Console.WriteLine($"Редактирование интерфейса: {SelectedInterface.InterfaceName}");
                // Реализация логики редактирования
            }
            return Task.CompletedTask;
        }

        private async Task LoadInterfacesAsync()
        {
            try
            {
                // Получаем коллекцию интерфейсов с сервера
                var interfaces = await _keeneticService!.GetWireGuardShowInterfacesAsync();

                // Создаём словарь для быстрого поиска по Id
                var newInterfacesDict = interfaces.ToDictionary(i => i.Id);

                // Удаляем интерфейсы, которых больше нет на сервере
                var toRemove = WireGuardShowInterfaces.Where(existing => !newInterfacesDict.ContainsKey(existing.Id)).ToList();
                foreach (var iface in toRemove)
                {
                    WireGuardShowInterfaces.Remove(iface);
                }

                // Добавляем новые и обновляем существующие
                foreach (var newInterface in interfaces)
                {
                    var existingInterface = WireGuardShowInterfaces.FirstOrDefault(i => i.Id == newInterface.Id);
                    if (existingInterface == null)
                    {
                        // Назначаем команду для переключения подключения
                        newInterface.ToggleConnectionCommand =
                            ReactiveCommand.CreateFromTask(() => ToggleConnectionAsync(newInterface));
                        WireGuardShowInterfaces.Add(newInterface);
                    }
                    else
                    {
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
        /// Обновляет данные существующего интерфейса.
        /// </summary>
        private void UpdateInterface(WireGuardShowInterface existingInterface, WireGuardShowInterface newData)
        {
            existingInterface.InterfaceName = newData.InterfaceName;
            existingInterface.Description   = newData.Description;
            existingInterface.Type          = newData.Type;
            existingInterface.Link          = newData.Link;
            existingInterface.Connected     = newData.Connected;
            existingInterface.State         = newData.State;
            existingInterface.Mtu           = newData.Mtu;
            existingInterface.Address       = newData.Address;
            existingInterface.Mask          = newData.Mask;
            existingInterface.Uptime        = newData.Uptime;
            existingInterface.WireGuard     = newData.WireGuard;
        }

        /// <summary>
        /// Переключает состояние интерфейса (включение/отключение VPN).
        /// </summary>
        private async Task ToggleConnectionAsync(WireGuardShowInterface wireGuardInterface)
        {
            _timer.Stop();

            if (_keeneticService == null) return;

            try
            {
                // Используем явное сравнение состояния
                if (wireGuardInterface.State.Equals("up", StringComparison.OrdinalIgnoreCase))
                {
                    await _keeneticService.SetInterfaceStateAsync(wireGuardInterface.Id, true);
                }
                else if (wireGuardInterface.State.Equals("down", StringComparison.OrdinalIgnoreCase))
                {
                    await _keeneticService.SetInterfaceStateAsync(wireGuardInterface.Id, false);
                }
                await _keeneticService.SystemConfigurationSaveAsync();
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
