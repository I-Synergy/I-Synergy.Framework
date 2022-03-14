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

        public Task ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            throw new NotImplementedException();
        }

        public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
        {
            throw new NotImplementedException();
        }

        public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
        {
            throw new NotImplementedException();
        }
    }
}
