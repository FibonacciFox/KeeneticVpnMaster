using KeeneticVpnMaster.ViewModels;
using Splat;

namespace KeeneticVpnMaster.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly KeeneticVpnMasterViewModel _mainViewModel;

        public NavigationService(KeeneticVpnMasterViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var viewModel = Locator.Current.GetService<TViewModel>();
            if (viewModel != null)
            {
                _mainViewModel.Content = viewModel;
            }
        }
    }
}