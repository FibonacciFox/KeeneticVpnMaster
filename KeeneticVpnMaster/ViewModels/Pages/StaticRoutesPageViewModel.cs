using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using iplist.opencck.org.parser;
using iplist.opencck.org.parser.Interfaces;
using KeeneticVpnMaster.Models;
using KeeneticVpnMaster.Services.Keenetic;
using ReactiveUI;
using Splat;
using SukiUI.Dialogs;

namespace KeeneticVpnMaster.ViewModels.Pages
{
    public class StaticRoutesPageViewModel : ViewModelBase
    {
        private readonly IIplistClient _iplistClient;
        private readonly IKeeneticService? _keeneticService = Locator.Current.GetService<IKeeneticService>();
        private readonly ISukiDialogManager? _sukiDialogManager = Locator.Current.GetService<ISukiDialogManager>();

        private ObservableCollection<StaticRouteItemViewModel> _routes;
        public ObservableCollection<StaticRouteItemViewModel> Routes
        {
            get => _routes;
            set => this.RaiseAndSetIfChanged(ref _routes, value);
        }

        // Свойства для интерфейсов WireGuard
        private ObservableCollection<string> _wireguardInterfaces = new ObservableCollection<string>();
        public ObservableCollection<string> WireguardInterfaces
        {
            get => _wireguardInterfaces;
            set => this.RaiseAndSetIfChanged(ref _wireguardInterfaces, value);
        }

        private string _selectedInterface = string.Empty;
        public string SelectedInterface
        {
            get => _selectedInterface;
            set => this.RaiseAndSetIfChanged(ref _selectedInterface, value);
        }

        // Команды для обновления, удаления и замены маршрутов
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> ReplaceCommand { get; }

        // Команды для выделения и снятия выделения со всех маршрутов
        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
        public ReactiveCommand<Unit, Unit> DeselectAllCommand { get; }

        // Команда для установки отмеченных маршрутов (Apply)
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }

        private double _progress;
        /// <summary>
        /// Значение прогресса выполнения (0–100)
        /// </summary>
        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public StaticRoutesPageViewModel()
        {
            _iplistClient = new IplistClient();
            _routes = new ObservableCollection<StaticRouteItemViewModel>();

            // Команда обновления – повторная загрузка данных
            UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await LoadData();
            });

            // Команда удаления маршрутов – отправка запроса с No = true
            DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Progress = 0;
                var selectedSites = new List<string>();
                foreach (var route in _routes)
                {
                    GetSelectedSitesRecursively(route, selectedSites);
                }

                if (!selectedSites.Any())
                {
                    _sukiDialogManager!.CreateDialog()
                        .OfType(NotificationType.Warning)
                        .WithTitle("Нет отмеченных маршрутов для удаления!")
                        .WithActionButton("Хорошо", _ => { }, true, "Flat", "Accent")
                        .Dismiss().ByClickingBackground()
                        .TryShow();
                    return;
                }

                int total = selectedSites.Count;
                int processed = 0;
                foreach (var site in selectedSites)
                {
                    try
                    {
                        var cidrData = await _iplistClient.GetCidrDataForSiteAsync(site);
                        foreach (var record in cidrData.Records)
                        {
                            var staticRouteModel = new StaticRouteModel
                            {
                                No = true, // Указываем, что маршрут нужно удалить
                                Network = record.Network,
                                Mask = record.SubnetMask,
                                Interface = SelectedInterface, // Используем выбранный интерфейс
                                Auto = true,
                                Comment = $"{site}"
                            };

                            await _keeneticService!.PostRequestAsync("ip/route", staticRouteModel);
                            await _keeneticService!.SystemConfigurationSaveAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка для сайта {site}: {ex.Message}");
                    }
                    processed++;
                    Progress = (processed * 100.0) / total;
                }

                _sukiDialogManager!.CreateDialog()
                    .OfType(NotificationType.Success)
                    .WithTitle("Удаление маршрутов завершено!")
                    .WithActionButton("Я доволен!", _ => { }, true, "Flat", "Accent")
                    .Dismiss().ByClickingBackground()
                    .TryShow();
                    Progress = 0;
            });

            // Команда замены – очищаем коллекцию и загружаем данные заново
            ReplaceCommand = ReactiveCommand.Create(() =>
            {
                _routes.Clear();
                _ = LoadData();
            });

            SelectAllCommand = ReactiveCommand.Create(() =>
            {
                foreach (var route in _routes)
                {
                    SelectRouteRecursively(route, true);
                }
            });

            DeselectAllCommand = ReactiveCommand.Create(() =>
            {
                foreach (var route in _routes)
                {
                    SelectRouteRecursively(route, false);
                }
            });

            // Команда установки маршрутов (Apply) – аналогичная логике удаления, но без No = true
            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Progress = 0;
                var selectedSites = new List<string>();
                foreach (var route in _routes)
                {
                    GetSelectedSitesRecursively(route, selectedSites);
                }

                if (!selectedSites.Any())
                {
                    _sukiDialogManager!.CreateDialog()
                        .OfType(NotificationType.Warning)
                        .WithTitle("Нет отмеченных маршрутов для загрузки!")
                        .WithActionButton("Хорошо", _ => { }, true, "Flat", "Accent")
                        .Dismiss().ByClickingBackground()
                        .TryShow();
                    return;
                }
                
                int total = selectedSites.Count;
                int processed = 0;
                foreach (var site in selectedSites)
                {
                    try
                    {
                        var cidrData = await _iplistClient.GetCidrDataForSiteAsync(site);
                        foreach (var record in cidrData.Records)
                        {
                            var staticRouteModel = new StaticRouteModel
                            {
                                // No остаётся false по умолчанию для установки маршрута
                                Network = record.Network,
                                Mask = record.SubnetMask,
                                Interface = SelectedInterface,
                                Auto = true,
                                Comment = $"{site}"
                            };

                            await _keeneticService!.PostRequestAsync("ip/route", staticRouteModel);
                            await _keeneticService!.SystemConfigurationSaveAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка для сайта {site}: {ex.Message}");
                    }
                    processed++;
                    Progress = (processed * 100.0) / total;
                }

                _sukiDialogManager!.CreateDialog()
                    .OfType(NotificationType.Success)
                    .WithTitle("Маршруты установлены!")
                    .WithActionButton("Я доволен!", _ => { }, true, "Flat", "Accent")
                    .Dismiss().ByClickingBackground()
                    .TryShow();
                    Progress = 0;
            });

            // Загружаем данные при инициализации
            _ = LoadData();
            LoadWireguardInterfaces();
        }

        private async Task LoadData()
        {
            try
            {
                var categories = await _iplistClient.GetCategoriesAsync();
                var items = categories.Select(cat =>
                {
                    var categoryItem = new StaticRouteItemViewModel(cat.Name);
                    var children = cat.Sites.Select(site => new StaticRouteItemViewModel(site, parent: categoryItem)).ToList();
                    return new StaticRouteItemViewModel(cat.Name, children);
                });

                Routes = new ObservableCollection<StaticRouteItemViewModel>(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private async void LoadWireguardInterfaces()
        {
            try
            {
                if (_keeneticService != null)
                {
                    var wgInterfaces = await _keeneticService.GetWireGuardShowInterfacesAsync();
                    WireguardInterfaces = new ObservableCollection<string>(wgInterfaces.Select(i => i.InterfaceName));
                    SelectedInterface = WireguardInterfaces.FirstOrDefault() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки интерфейсов WireGuard: " + ex.Message);
            }
        }

        private void SelectRouteRecursively(StaticRouteItemViewModel route, bool select)
        {
            route.IsChecked = select;
            foreach (var child in route.Children)
            {
                SelectRouteRecursively(child, select);
            }
        }

        /// <summary>
        /// Рекурсивно собирает все отмеченные сайты (листья без детей) в список.
        /// </summary>
        private void GetSelectedSitesRecursively(StaticRouteItemViewModel route, List<string> selectedSites)
        {
            if (!route.Children.Any())
            {
                if (route.IsChecked)
                    selectedSites.Add(route.Name);
            }
            else
            {
                foreach (var child in route.Children)
                {
                    GetSelectedSitesRecursively(child, selectedSites);
                }
            }
        }
    }
}
