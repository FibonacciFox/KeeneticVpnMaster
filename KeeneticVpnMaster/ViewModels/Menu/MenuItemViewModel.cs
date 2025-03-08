using System;
using System.Reactive;
using ReactiveUI;

namespace KeeneticVpnMaster.ViewModels.Menu;

public class MenuItemViewModel : ViewModelBase
{
    public string Header { get; }
    public string Icon { get; }
    public Type ViewModelType { get; }
    public ReactiveCommand<Unit, Unit> NavigateCommand { get; }

    public MenuItemViewModel(string header, string icon, Type viewModelType, Action<Type> navigateAction)
    {
        Header = header;
        Icon = icon;
        ViewModelType = viewModelType;
        NavigateCommand = ReactiveCommand.Create(() => navigateAction(viewModelType));
    }
}