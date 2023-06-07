using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Event handler when backstack is changed.
        /// </summary>
        event EventHandler BackStackChanged;

        /// <summary>
        /// Frame to navigate.
        /// </summary>
        object Frame { get; set; }

        bool CanGoBack { get; }
        bool CanGoForward { get; }

        Task GoBackAsync();
        Task GoForwardAsync();
        Task CleanBackStackAsync();

        /// <summary>
        /// Navigates to a specified viewmodel asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="parameter"></param>
        /// <param name="navigateBack"></param>
        /// <returns></returns>
        Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel;

        /// <summary>
        /// Navigates to a specified viewmodel asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="navigateBack"></param>
        /// <returns></returns>
        Task NavigateAsync<TViewModel>(object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel;

        /// <summary>
        /// Navigates to the modal viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task NavigateModalAsync<TViewModel>(object parameter = null) where TViewModel : class, IViewModel;

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

        /// <summary>
        /// open blade as an asynchronous operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel);

        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);
    }
}
