﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Controls;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Application = Microsoft.UI.Xaml.Application;
using Style = Microsoft.UI.Xaml.Style;
using Setter = Microsoft.UI.Xaml.Setter;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public partial class DialogService
    {
        private Window _activeDialog = null;

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

            if(Application.Current is BaseApplication baseApplication)
                dialog.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.SecondaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
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
            where TViewModel : IViewModelDialog<TEntity>
            {
                if (viewmodel is null)
                    viewmodel = (IViewModelDialog<TEntity>)ServiceLocator.Default.GetInstance(typeof(TViewModel));

                return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(typeof(TWindow)), viewmodel);
            }

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
            if(Application.Current is BaseApplication baseApplication)
                dialog.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

            dialog.DataContext = viewmodel;

            dialog.PrimaryButtonCommand = viewmodel.Submit_Command;
            dialog.SecondaryButtonCommand = viewmodel.Close_Command;
            dialog.CloseButtonCommand = viewmodel.Close_Command;

            dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
            dialog.SecondaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];

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