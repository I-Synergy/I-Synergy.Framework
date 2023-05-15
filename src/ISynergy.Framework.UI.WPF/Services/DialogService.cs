using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.UI.Services.Base;
using System.Windows;
using MessageBoxButton = ISynergy.Framework.Mvvm.Enumerations.MessageBoxButton;
using MessageBoxResult = ISynergy.Framework.Mvvm.Enumerations.MessageBoxResult;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public class DialogService : BaseDialogService
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
        public override Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
        {
            var button = System.Windows.MessageBoxButton.OK;

            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    button = System.Windows.MessageBoxButton.OKCancel;
                    break;
                case MessageBoxButton.YesNoCancel:
                    button = System.Windows.MessageBoxButton.YesNoCancel;
                    break;
                case MessageBoxButton.YesNo:
                    button = System.Windows.MessageBoxButton.YesNo;
                    break;
            }

            var result = System.Windows.MessageBox.Show(
                Application.Current.MainWindow,
                message,
                title,
                button,
                System.Windows.MessageBoxImage.Information,
                System.Windows.MessageBoxResult.Cancel);

            switch (result)
            {
                case System.Windows.MessageBoxResult.OK:
                    return Task.FromResult(MessageBoxResult.OK);
                case System.Windows.MessageBoxResult.Cancel:
                    return Task.FromResult(MessageBoxResult.Cancel);
                case System.Windows.MessageBoxResult.Yes:
                    return Task.FromResult(MessageBoxResult.Yes);
                case System.Windows.MessageBoxResult.No:
                    return Task.FromResult(MessageBoxResult.No);
                default:
                    return Task.FromResult(MessageBoxResult.None);
            }
        }
    }
}
