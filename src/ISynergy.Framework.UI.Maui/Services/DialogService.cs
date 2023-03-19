using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Services.Base;
using Mopups.Interfaces;
using Mopups.Pages;
using Application = Microsoft.Maui.Controls.Application;

namespace ISynergy.Framework.UI.Services
{
    internal class DialogService : BaseDialogService
    {
        private readonly IPopupNavigation _popupNavigation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="popupNavigation"></param>
        /// <param name="languageService">The language service.</param>
        /// <param name="dispatcherService"></param>
        public DialogService(IPopupNavigation popupNavigation, ILanguageService languageService, IDispatcherService dispatcherService)
            : base(languageService, dispatcherService)
        {
            _popupNavigation = popupNavigation;
        }

        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>MessageBoxResult.</returns>
        public override async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
        {
            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    if (await Application.Current.MainPage.DisplayAlert(
                        title,
                        message,
                        _languageService.GetString("Ok"),
                        _languageService.GetString("Cancel")))
                        return MessageBoxResult.OK;
                    else
                        return MessageBoxResult.Cancel;
                case MessageBoxButton.YesNo:
                    if (await Application.Current.MainPage.DisplayAlert(
                        title,
                        message,
                        _languageService.GetString("Yes"),
                        _languageService.GetString("No")))
                        return MessageBoxResult.Yes;
                    else
                        return MessageBoxResult.No;
                default:
                    await Application.Current.MainPage.DisplayAlert(
                        title,
                        message,
                        _languageService.GetString("Ok"));

                    return MessageBoxResult.OK;
            }
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
