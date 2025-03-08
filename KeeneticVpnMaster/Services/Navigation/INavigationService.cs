using KeeneticVpnMaster.ViewModels;

namespace KeeneticVpnMaster.Services.Navigation
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    }
}