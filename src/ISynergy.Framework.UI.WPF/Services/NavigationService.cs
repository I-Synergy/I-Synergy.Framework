using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Window = ISynergy.Framework.UI.Controls.Window;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="IBaseNavigationService" />
    /// </summary>
    /// <seealso cref="IBaseNavigationService" />
    public class NavigationService : BaseNavigationService, INavigationService
    {
        /// <summary>
        /// The frame
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// The is shown
        /// </summary>
        private bool _isShown = false;

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public object Frame
        {
            get => _frame ??= (Frame)Application.Current.MainWindow.Content;
            set => _frame = (Frame)value;
        }

        /// <summary>
        /// Gets the registered windows.
        /// </summary>
        /// <value>The registered windows.</value>
        public List<Window> RegisteredWindows { get; internal set; } = new List<Window>();

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => _frame.CanGoBack;
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public bool CanGoForward => _frame.CanGoForward;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="context"></param>
        public NavigationService(IContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Goes the back.
        /// </summary>
        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public void GoForward()
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }

        /// <summary>
        /// navigate as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}.</exception>
        public void Navigate<TViewModel>(object parameter = null)
            where TViewModel : class, IViewModel
        {
            var viewmodel = parameter is IViewModel instance ? instance : _context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();
            var page = WPFAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewmodel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewmodel.GetViewFullName()}.");

            // Check if actual page is the same as destination page.
            if (_frame.Content is not null && _frame.Content.GetType().Equals(page))
                return;

            if (parameter is IViewModel)
            {
                _frame.NavigateToView(page, viewmodel);
            }
            else
            {
                if (page.GetInterfaces(true).Any(q => q == typeof(IView)) && parameter is not null && !string.IsNullOrEmpty(parameter.ToString()))
                {
                    Type genericPropertyType = null;

                    // Has class GenericTypeArguments?
                    if (viewmodel.GetType().GenericTypeArguments.Any())
                        genericPropertyType = viewmodel.GetType().GetGenericArguments().First();

                    // Has BaseType GenericTypeArguments?
                    else if (viewmodel.GetType().BaseType is Type baseType && baseType.GenericTypeArguments.Any())
                        genericPropertyType = baseType.GetGenericArguments().First();

                    if (genericPropertyType is not null && parameter.GetType() == genericPropertyType)
                    {
                        var genericInterfaceType = typeof(IViewModelSelectedItem<>).MakeGenericType(genericPropertyType);

                        // Check if instance implements genericInterfaceType.
                        if (genericInterfaceType.IsInstanceOfType(viewmodel.GetType()) && viewmodel.GetType().GetMethod("SetSelectedItem") is MethodInfo method)
                            method.Invoke(viewmodel, new[] { parameter });
                    }
                }

                _frame.NavigateToView(page, viewmodel);
            }
        }

        /// <summary>
        /// get navigation blade as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>IView.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewModelKey}. - viewModel</exception>
        /// <exception cref="ArgumentException">Instance could not be created from {viewModelKey}</exception>
        private async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
        {
            var page = WPFAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewModel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewModel.GetViewFullName()}.");

            if (_context.ScopedServices.ServiceProvider.GetRequiredService(page) is ISynergy.Framework.UI.Controls.View view)
            {
                view.ViewModel = viewModel;
                await viewModel.InitializeAsync();
                return view;
            }

            throw new Exception($"Instance could not be created from {viewModel.GetViewFullName()}");
        }

        /// <summary>
        /// open blade as an asynchronous operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
        {
            Argument.IsNotNull(owner);

            if (viewmodel is IViewModelBlade bladeVm)
            {
                bladeVm.Owner = owner;
                bladeVm.Closed += Viewmodel_Closed;

                var view = await GetNavigationBladeAsync(bladeVm);

                if (!owner.Blades.Any(a => a.GetType().FullName.Equals(view.GetType().FullName)))
                {
                    foreach (var blade in owner.Blades)
                    {
                        blade.IsEnabled = false;
                    }

                    owner.Blades.Add(view);
                }

                owner.IsPaneVisible = true;
            }
        }

        /// <summary>
        /// Handles the Closed event of the Viewmodel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Viewmodel_Closed(object sender, EventArgs e)
        {
            if (sender is IViewModelBlade viewModel)
                RemoveBlade(viewModel.Owner, viewModel);
        }

        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
        {
            Argument.IsNotNull(owner);

            if (viewmodel is IViewModelBlade bladeVm)
            {
                if (owner.Blades is not null)
                {
                    if (owner.Blades.Remove(
                        owner.Blades
                            .FirstOrDefault(q =>
                                q.ViewModel == bladeVm &&
                                ((IViewModelBlade)q.ViewModel).Owner == bladeVm.Owner))
                        )
                    {
                        if (owner.Blades.Count < 1)
                            owner.IsPaneVisible = false;
                        else if (owner.Blades.Last() is IView blade)
                            blade.IsEnabled = true;
                    }
                }
            }
        }

        public override Task NavigateAsync<TViewModel>()
        {
            this.Navigate<TViewModel>();
            return Task.CompletedTask;
        }

        public override Task NavigateAsync<TViewModel>(object parameter)
        {
            this.Navigate<TViewModel>(parameter);
            return Task.CompletedTask;
        }

        public Task NavigateAsync(Type viewModel) =>
            throw new NotImplementedException();

        public Task CleanBackStackAsync()
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in ((Frame)_frame).BackStack.EnsureNotNull())
                {
                    ((Frame)_frame).RemoveBackEntry();
                }
            }).Task;
        }

        public override Task ReplaceMainWindowAsync<TView>()
        {
            if (_context.ScopedServices.ServiceProvider.GetRequiredService<TView>() is Page page)
                Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(TView)}' is not of type of Page.");

            return Task.CompletedTask;
        }

        public override Task ReplaceMainFrameAsync<TView>()
        {
            if (_context.ScopedServices.ServiceProvider.GetRequiredService<TView>() is Page page)
                Application.Current.Dispatcher.Invoke(() => _frame.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(TView)}' is not of type of Page.");

            return Task.CompletedTask;
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
                window.Owner = Application.Current.MainWindow;

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
    }
}
