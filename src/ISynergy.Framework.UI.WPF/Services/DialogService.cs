using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services.Base;
using MessageBoxButton = ISynergy.Framework.Mvvm.Enumerations.MessageBoxButton;
using MessageBoxResult = ISynergy.Framework.Mvvm.Enumerations.MessageBoxResult;
using Window = ISynergy.Framework.UI.Controls.Window;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public class DialogService : BaseDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// The is shown
        /// </summary>
        private bool _isShown = false;

        /// <summary>
        /// Gets the registered windows.
        /// </summary>
        /// <value>The registered windows.</value>
        public List<Window> RegisteredWindows { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="languageService">The language service.</param>
        /// <param name="dispatcherService"></param>
        public DialogService(IServiceProvider serviceProvider, ILanguageService languageService, IDispatcherService dispatcherService)
            : base(languageService, dispatcherService)
        {
            _serviceProvider = serviceProvider;

            RegisteredWindows = new List<Window>();
        }

        public override Task CloseDialogAsync(IWindow dialog)
        {
            if (dialog is Window window)
                window.Close();

            return Task.CompletedTask;
        }

        public override async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
        {
            if (dialog is Window window)
            {
                window.ViewModel = viewmodel;

                viewmodel.Closed += async (sender, e) => await CloseDialogAsync(window);
                viewmodel.Submitted += async (sender, e) => await CloseDialogAsync(window);

                if (!viewmodel.IsInitialized)
                    await viewmodel.InitializeAsync();

                RegisteredWindows.Add(window);

                if (_isShown)
                    return;

                _isShown = true;

                for (var i = 0; i < RegisteredWindows.Count(q => q.Equals(window)); i++)
                {
                    await RegisteredWindows[i].ShowAsync<TEntity>();
                    RegisteredWindows.Remove(RegisteredWindows[i]);
                    i--;
                }

                _isShown = false;
            }
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
