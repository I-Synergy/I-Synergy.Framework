using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Services
{
    internal class NavigationService : INavigationService
    {
        public object Frame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanGoBack => throw new NotImplementedException();

        public bool CanGoForward => throw new NotImplementedException();

        /// <summary>
        /// clean back stack as an asynchronous operation.
        /// </summary>
        public Task CleanBackStackAsync() => Task.CompletedTask;

        public void Configure(string key, Type pageType)
        {
        }

        public void GoBack() => throw new NotImplementedException();

        public void GoForward() => throw new NotImplementedException();

        public Task NavigateAsync<TViewModel>() where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true);

        public Task NavigateAsync<TViewModel>(object parameter) where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true, new Dictionary<string, object> { { GenericConstants.Parameter, parameter } });

        public Task NavigateAsync(Type viewModel) =>
            Shell.Current.GoToAsync(viewModel.GetViewModelFullName(), true);

        public void OpenBlade(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

        public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();
    }
}
