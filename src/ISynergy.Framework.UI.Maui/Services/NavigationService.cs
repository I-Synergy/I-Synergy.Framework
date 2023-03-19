using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    internal class NavigationService : INavigationService
    {
        public Task NavigateAsync<TViewModel>() where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true);

        public Task NavigateAsync<TViewModel>(object parameter) where TViewModel : class, IViewModel =>
            Shell.Current.GoToAsync(typeof(TViewModel).GetViewModelFullName(), true, new Dictionary<string, object> { { GenericConstants.Parameter, parameter } });

        public Task ReplaceMainFrameAsync<T>() where T : IView => throw new NotImplementedException();

        public Task ReplaceMainWindowAsync<T>() where T : IView
        {
            if (ServiceLocator.Default.GetInstance<T>() is Page page)
                Application.Current.MainPage.Dispatcher.Dispatch(() =>
                {
                    Application.Current.MainPage = new NavigationPage(page);
                });
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");

            return Task.CompletedTask;
        }
    }
}
