using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Mopups.Interfaces;
using Mopups.Pages;

namespace ISynergy.Framework.UI.Services
{
    internal class NavigationService : BaseNavigationService
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
        /// Navigates to the viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override Task NavigateAsync<TViewModel>(object parameter = null) =>
            Shell.Current.Navigation.PushViewModelAsync<TViewModel>(parameter);

        /// <summary>
        /// Navigates to the modal viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override Task NavigateModalAsync<TViewModel>(object parameter = null)
        {
            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                Application.Current.MainPage = NavigationExtensions.CreatePage<TViewModel>(parameter);
            });

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

        public override object Frame { get; set; }
    }
}
