using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using System;
using System.Threading.Tasks;


namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public partial class DialogService
    {
        private System.Windows.Window _activeDialog = null;
        
        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        /// <returns>MessageBoxResult.</returns>
        public virtual Task<MessageBoxResult> ShowMessageAsync(
            string message,
            string title = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            var result = System.Windows.MessageBox.Show(
                message,
                title,
                (System.Windows.MessageBoxButton)buttons,
                (System.Windows.MessageBoxImage)image,
                (System.Windows.MessageBoxResult)MessageBoxResult.Cancel);

            return Task.FromResult((MessageBoxResult)result);
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

            return CreateDialogAsync((System.Windows.Window)ServiceLocator.Default.GetInstance(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((System.Windows.Window)ServiceLocator.Default.GetInstance(window.GetType()), viewmodel);

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((System.Windows.Window)ServiceLocator.Default.GetInstance(type), viewmodel);

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        private async Task CreateDialogAsync<TEntity>(System.Windows.Window dialog, IViewModelDialog<TEntity> viewmodel)
        {
            dialog.DataContext = viewmodel;

            viewmodel.Submitted += (sender, e) => CloseDialog(dialog);
            viewmodel.Cancelled += (sender, e) => CloseDialog(dialog);
            viewmodel.Closed += (sender, e) => CloseDialog(dialog);

            if (!viewmodel.IsInitialized)
                await viewmodel.InitializeAsync();

            await OpenDialogAsync(dialog);
        }

        private Task<bool?> OpenDialogAsync(System.Windows.Window dialog)
        {
            if (_activeDialog is not null)
                CloseDialog(_activeDialog);

            _activeDialog = dialog;
            _activeDialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            if (System.Windows.Application.Current.MainWindow != null && System.Windows.Application.Current.MainWindow.IsLoaded) { }
                _activeDialog.Owner = System.Windows.Application.Current.MainWindow;

            return Task.FromResult(_activeDialog.ShowDialog());
        }

        private void CloseDialog(System.Windows.Window dialog)
        {
            _activeDialog.Hide();
            _activeDialog = null;
        }
    }
}
