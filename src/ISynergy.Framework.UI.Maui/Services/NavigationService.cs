using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Mopups.Interfaces;
using Mopups.Pages;

namespace ISynergy.Framework.UI.Services
{
    internal class NavigationService : BaseNavigationService, INavigationService
    {
        private readonly IPopupNavigation _popupNavigation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="popupNavigation"></param>
        public NavigationService(
            IContext context,
            IPopupNavigation popupNavigation)
            : base(context)
        {
            _popupNavigation = popupNavigation;
        }

        /// <summary>
        /// Navigates to the viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public override Task NavigateAsync<TViewModel>() =>
            Shell.Current.Navigation.PushViewModelAsync<TViewModel>();
            
        /// <summary>
        /// Navigates to the viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override Task NavigateAsync<TViewModel>(object parameter) =>
            Shell.Current.Navigation.PushViewModelAsync<TViewModel>(parameter);

        /// <summary>
        /// Replaces the main frame asynchronous.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task ReplaceMainFrameAsync<TView>() => 
            throw new NotImplementedException();

        /// <summary>
        /// Replaces the main window asynchronous.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public override Task ReplaceMainWindowAsync<TView>()
        {
            if (_context.ScopedServices.ServiceProvider.GetRequiredService<TView>() is Page page)
                Application.Current.MainPage.Dispatcher.Dispatch(() =>
                {
                    Application.Current.MainPage = new NavigationPage(page);
                });
            else
                throw new InvalidCastException($"Implementation of '{nameof(TView)}' is not of type of Page.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="window"></param>
        /// <param name="viewmodel"></param>
        /// <returns></returns>
        public override async Task CreateDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
        {
            window.ViewModel = viewmodel;

            viewmodel.Closed += async (sender, e) => await CloseDialogAsync(window);

            if (!viewmodel.IsInitialized)
                await viewmodel.InitializeAsync();

            if (window is Window dialog)
                await _popupNavigation.PushAsync(dialog);
        }

        /// <summary>
        /// Closes dialog window.
        /// </summary>
        /// <param name="dialog"></param>
        public override Task CloseDialogAsync(IWindow dialog)
        {
            if (dialog is PopupPage page &&
                _popupNavigation.PopupStack is not null &&
                _popupNavigation.PopupStack.Count > 0 &&
                _popupNavigation.PopupStack.Contains(page))
                return _popupNavigation.RemovePageAsync(page);

            return Task.CompletedTask;
        }
    }
}
