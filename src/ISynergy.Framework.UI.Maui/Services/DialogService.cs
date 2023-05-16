using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Services.Base;
using Application = Microsoft.Maui.Controls.Application;

namespace ISynergy.Framework.UI.Services
{
    internal class DialogService : BaseDialogService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public DialogService(ILanguageService languageService)
            : base(languageService)
        {
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
    }
}
