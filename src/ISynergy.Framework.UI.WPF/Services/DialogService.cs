using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
        private readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="dispatcherService"></param>
        /// <param name="languageService">The language service.</param>
        public DialogService(
            IDispatcherService dispatcherService,
            ILanguageService languageService)
            : base(languageService)
        {
            _dispatcherService = dispatcherService;
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
            var result = MessageBoxResult.None;

            _dispatcherService.Invoke(() =>
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

                var dialog = MessageBox.Show(
                    Application.Current.MainWindow,
                    message,
                    title,
                    button,
                    MessageBoxImage.Information,
                    System.Windows.MessageBoxResult.Cancel);

                result = dialog switch
                {
                    System.Windows.MessageBoxResult.None => MessageBoxResult.None,
                    System.Windows.MessageBoxResult.OK => MessageBoxResult.OK,
                    System.Windows.MessageBoxResult.Cancel => MessageBoxResult.Cancel,
                    System.Windows.MessageBoxResult.Yes => MessageBoxResult.Yes,
                    System.Windows.MessageBoxResult.No => MessageBoxResult.No,
                    _ => MessageBoxResult.None,
                };
            });

            return Task.FromResult(result);
        }
    }
}
