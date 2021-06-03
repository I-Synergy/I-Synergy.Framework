using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml.Controls;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        private readonly ILanguageService _languageService;

        private Window _activeDialog = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public DialogService(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "") =>
            ShowMessageAsync(error.Message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);

        /// <summary>
        /// Shows the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information);

        /// <summary>
        /// Shows the warning asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);

        /// <summary>
        /// Shows the greeting asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public Task ShowGreetingAsync(string name)
        {
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Night"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Morning"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Afternoon"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Evening"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        /// <returns>MessageBoxResult.</returns>
        public virtual async Task<MessageBoxResult> ShowMessageAsync(
            string message,
            string title = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            var dialog = new Window()
            {
                Title = title,
                Content = message
            };

            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    break;
                case MessageBoxButton.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");
                    break;
            }

            if(await OpenDialogAsync(dialog) is ContentDialogResult result)
            {
                switch (buttons)
                {
                    case MessageBoxButton.OKCancel:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.OK;
                            default:
                                return MessageBoxResult.Cancel;
                        }
                    case MessageBoxButton.YesNoCancel:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.Yes;
                            case ContentDialogResult.Secondary:
                                return MessageBoxResult.No;
                            default:
                                return MessageBoxResult.Cancel;
                        }
                    case MessageBoxButton.YesNo:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.Yes;
                            default:
                                return MessageBoxResult.No;
                        }
                    default:
                        return MessageBoxResult.OK;
                }
            }

            return MessageBoxResult.Cancel;
        }


        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity> =>
            CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(typeof(TWindow)), viewmodel ??= (IViewModelDialog<TEntity>)ServiceLocator.Default.GetInstance(typeof(TViewModel)));

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(window.GetType()), viewmodel);

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(type), viewmodel);

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        private async Task CreateDialogAsync<TEntity>(Window dialog, IViewModelDialog<TEntity> viewmodel)
        {
            dialog.DataContext = viewmodel;
            dialog.PrimaryButtonCommand = viewmodel.Submit_Command;
            dialog.SecondaryButtonCommand = viewmodel.Close_Command;
            dialog.CloseButtonCommand = viewmodel.Close_Command;

            viewmodel.Submitted += (sender, e) => CloseDialog(dialog);
            viewmodel.Cancelled += (sender, e) => CloseDialog(dialog);
            viewmodel.Closed += (sender, e) => CloseDialog(dialog);

            if (!viewmodel.IsInitialized)
                await viewmodel.InitializeAsync();

            await OpenDialogAsync(dialog);
        }

        private Task<ContentDialogResult> OpenDialogAsync(Window dialog)
        {
            if(_activeDialog is not null)
                CloseDialog(_activeDialog);

            _activeDialog = dialog;
            return _activeDialog.ShowAsync().AsTask();
        }

        private void CloseDialog(Window dialog)
        {
            dialog.Close();
            _activeDialog = null;
        }
    }
}
