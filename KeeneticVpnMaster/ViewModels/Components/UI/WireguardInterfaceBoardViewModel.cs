using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Layout;
using Avalonia.Threading;
using KeeneticVpnMaster.Models;
using KeeneticVpnMaster.Services.Keenetic;
using ReactiveUI;
using Splat;
using SukiUI.Dialogs;

namespace KeeneticVpnMaster.ViewModels.Components.UI
{
    public class WireguardInterfaceBoardViewModel : ViewModelBase
    {
        private readonly IKeeneticService? _keeneticService = Locator.Current.GetService<IKeeneticService>();
        private readonly ISukiDialogManager? _dialogManager = Locator.Current.GetService<ISukiDialogManager>();
        
        private readonly DispatcherTimer _timer;

        //Коллекция всех интерфейсов на сервере с общей информацией о каждом интерфейсе WireGuard
        public ObservableCollection<WireGuardInterfaceInfo> WireGuardShowInterfaces { get; set; } = new();

        //командd редактирования интерфейса WireGuard
        public ICommand EditInterfaceCommand { get; }
        
        //командd удаления интерфейса WireGuard
        public ICommand RemoveInterfaceCommand { get; }

        public WireguardInterfaceBoardViewModel()
        {
            // Инициализируем команду редактирования\удаления интерфейса
            EditInterfaceCommand = ReactiveCommand.Create<WireGuardInterfaceInfo>(EditInterface);
            RemoveInterfaceCommand = ReactiveCommand.Create<WireGuardInterfaceInfo>(RemoveInterface);

            // Настраиваем таймер для периодической загрузки интерфейсов
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += async (s, e) => await LoadInterfacesAsync();
            _timer.Start();
        }
        
        /// <summary>
        /// Обрабатывает команду редактирования интерфейса.
        /// </summary>
        private void EditInterface(WireGuardInterfaceInfo? selectedInterface)
        {
            if (selectedInterface != null)
            {
                Console.WriteLine($"Edit Interface: {selectedInterface.InterfaceName}");
            }
            else
            {
                Console.WriteLine("Выбранный интерфейс не найден.");
            }
        }
        
        /// <summary>
        /// Обрабатывает команду удаления интерфейса.
        /// </summary>
        private void RemoveInterface(WireGuardInterfaceInfo? selectedInterface)
        {
            if (selectedInterface != null)
            {
                _dialogManager!.CreateDialog()
                    .OfType(NotificationType.Warning)
                    .WithTitle($"Удаление интерфейса {selectedInterface.Description}!")
                    .WithContent(new TextBlock()
                    {
                        Text = $"Вы уверены, что хотите удалить {selectedInterface.Description}?",
                        HorizontalAlignment = HorizontalAlignment.Center
                    })
                    .WithActionButton("Удалить", _ =>
                    {
                        _keeneticService!.DeleteInterface(selectedInterface.InterfaceName);
                    }, true, "Flat", "Accent")
                    .WithActionButton("Отмена", _ => { }, true, "Flat")
                    .Dismiss().ByClickingBackground()
                    .TryShow();
            }
            else
            {
                Console.WriteLine("Выбранный интерфейс не найден.");
            }
        }

        

        /// <summary>
        /// Загружает список интерфейсов WireGuard.
        /// </summary>
        private async Task LoadInterfacesAsync()
        {
            try
            {
                if (_keeneticService == null) return;

                // Получаем актуальный список интерфейсов
                var interfaces = await _keeneticService.GetWireGuardShowInterfacesAsync();

                // Создаём словарь интерфейсов по их Id
                var newInterfacesDict = interfaces.ToDictionary(i => i.Id);

                // Удаляем интерфейсы, которых больше нет на сервере
                var toRemove = WireGuardShowInterfaces
                    .Where(existing => !newInterfacesDict.ContainsKey(existing.Id))
                    .ToList();

                foreach (var iface in toRemove)
                {
                    WireGuardShowInterfaces.Remove(iface);
                }

                // Добавляем новые интерфейсы и обновляем существующие
                foreach (var newInterface in interfaces)
                {
                    var existingInterface = WireGuardShowInterfaces.FirstOrDefault(i => i.Id == newInterface.Id);
                    if (existingInterface == null)
                    {
                        // Назначаем команду для переключения состояния подключения
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
        private void UpdateInterface(WireGuardInterfaceInfo existingInterfaceInfo, WireGuardInterfaceInfo newData)
        {
            existingInterfaceInfo.InterfaceName = newData.InterfaceName;
            existingInterfaceInfo.Description   = newData.Description;
            existingInterfaceInfo.Type          = newData.Type;
            existingInterfaceInfo.Link          = newData.Link;
            existingInterfaceInfo.Connected     = newData.Connected;
            existingInterfaceInfo.State         = newData.State;
            existingInterfaceInfo.Mtu           = newData.Mtu;
            existingInterfaceInfo.Address       = newData.Address;
            existingInterfaceInfo.Mask          = newData.Mask;
            existingInterfaceInfo.Uptime        = newData.Uptime;
            existingInterfaceInfo.WireGuard     = newData.WireGuard;
        }

        /// <summary>
        /// Переключает состояние интерфейса (включение/отключение VPN).
        /// </summary>
        private async Task ToggleConnectionAsync(WireGuardInterfaceInfo wireGuardInterfaceInfo)
        {
            _timer.Stop();

            if (_keeneticService == null) return;

            try
            {
                // Используем явное сравнение состояния
                if (wireGuardInterfaceInfo.State.Equals("up", StringComparison.OrdinalIgnoreCase))
                {
                    await _keeneticService.SetInterfaceStateAsync(wireGuardInterfaceInfo.Id, true);
                }
                else if (wireGuardInterfaceInfo.State.Equals("down", StringComparison.OrdinalIgnoreCase))
                {
                    await _keeneticService.SetInterfaceStateAsync(wireGuardInterfaceInfo.Id, false);
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
