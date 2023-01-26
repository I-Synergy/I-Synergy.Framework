using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Services
{
    internal class NavigationService : INavigationService
    {
        public Task NavigateAsync<TViewModel>() where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true);

        public Task NavigateAsync<TViewModel>(object parameter) where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true, new Dictionary<string, object> { { GenericConstants.Parameter, parameter } });

        public Task NavigateAsync(Type viewModel) =>
            Shell.Current.GoToAsync(viewModel.GetViewModelFullName(), true);
    }
}
