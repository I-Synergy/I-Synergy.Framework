using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services.Base
{
    public interface IBaseNavigationService
    {
        /// <summary>
        /// Navigates the asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <returns>Task</returns>
        Task NavigateAsync<TViewModel>()
            where TViewModel : class, IViewModel;

        Task NavigateAsync<TViewModel>(object parameter)
            where TViewModel : class, IViewModel;

        Task ReplaceMainWindowAsync<T>() where T : IView;
        Task ReplaceMainFrameAsync<T>() where T : IView;

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <returns>Task{TEntity}.</returns>
        Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>;

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="e">The selected item.</param>
        /// <returns>Task{TEntity}.</returns>
        Task ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>;

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel);

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel);
    }
}
