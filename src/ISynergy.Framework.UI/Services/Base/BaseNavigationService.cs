using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.UI.Services.Base
{
    public abstract class BaseNavigationService : INavigationService
    {
        protected readonly IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNavigationService"/> class.
        /// </summary>
        /// <param name="context"></param>
        protected BaseNavigationService(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));
            return CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="e">The selected item.</param>
        /// <returns>Task{TEntity}.</returns>
        public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));
            await viewmodel.SetSelectedItemAsync(e);
            await CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(window.GetType()), viewmodel);

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(type), viewmodel);

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="window"></param>
        /// <param name="viewmodel"></param>
        /// <returns></returns>
        public abstract Task CreateDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel);

        /// <summary>
        /// Closes dialog window.
        /// </summary>
        /// <param name="dialog"></param>
        public abstract Task CloseDialogAsync(IWindow dialog);

        /// <summary>
        /// Navigates to a specified viewmodel asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task NavigateAsync<TViewModel>(object parameter = null) where TViewModel : class, IViewModel;

        /// <summary>
        /// Navigates to the modal viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task NavigateModalAsync<TViewModel>(object parameter = null) where TViewModel : class, IViewModel;

        public virtual object Frame { get; set; }
        public virtual bool CanGoBack { get; }
        public virtual bool CanGoForward { get; }

        public virtual void GoBack() { }
        public virtual void GoForward() { }
        public virtual Task CleanBackStackAsync() => Task.CompletedTask;

        /// <summary>
        /// open blade as an asynchronous operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public virtual Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel) => Task.CompletedTask;

        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public virtual void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel) { }
    }
}
