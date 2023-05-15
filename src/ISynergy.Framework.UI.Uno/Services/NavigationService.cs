using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="INavigationService" />
    /// </summary>
    /// <seealso cref="INavigationService" />
    public class NavigationService : BaseNavigationService, INavigationService
    {
        private Window _activeDialog = null;

        /// <summary>
        /// The frame
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public object Frame
        {
            get => _frame ??= (Frame)Microsoft.UI.Xaml.Window.Current.Content;
            set => _frame = (Frame)value;
        }

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
        /// get navigation blade as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>IView.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure? - viewModel</exception>
        /// <exception cref="ArgumentException">Instance could not be created from {viewModelKey}</exception>
        private async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
        {
            var page = default(Type);

            if (viewModel is ISelectionViewModel selectionBlade)
            {
                page = typeof(SelectionView);
            }
            else
            {
                page = WindowsAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewModel.GetViewFullName()));
            }

            if (page is null)
                throw new Exception($"Page not found: {viewModel.GetViewFullName()}.");

            if (_context.ScopedServices.ServiceProvider.GetRequiredService(page) is ISynergy.Framework.UI.Controls.View view)
            {
                view.ViewModel = viewModel;

                if (!viewModel.IsInitialized)
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

        public override Task NavigateAsync<TViewModel>() =>
            NavigateAsync<TViewModel>(null);


        /// <summary>
        /// navigate as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
        public override async Task NavigateAsync<TViewModel>(object parameter)
        {
            var viewmodel = parameter is IViewModel instance ? instance : _context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();
            var page = WindowsAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewmodel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewmodel.GetViewFullName()}.");

            // Check if actual page is the same as destination page.
            if (_frame.Content is not null && _frame.Content.GetType().Equals(page))
                return;

            if (parameter is IViewModel)
            {
                _frame.Navigate(page, viewmodel);
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

                _frame.Navigate(page, viewmodel);
            }

            if (!viewmodel.IsInitialized)
                await viewmodel.InitializeAsync();
        }

        public Task CleanBackStackAsync()
        {
            _frame.BackStack.Clear();
            return Task.CompletedTask;
        }

        public override Task ReplaceMainWindowAsync<T>()
        {
            if (_context.ScopedServices.ServiceProvider.GetRequiredService<T>() is Page page && Application.Current is BaseApplication baseApplication)
                DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Normal, () => baseApplication.MainWindow.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");

            return Task.CompletedTask;
        }

        public override Task ReplaceMainFrameAsync<T>()
        {
            if (_context.ScopedServices.ServiceProvider.GetRequiredService<T>() is Page page && Application.Current is BaseApplication baseApplication)
                DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Normal, () => _frame.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        public override async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
        {
            if (dialog is Window window)
            {
                if (Application.Current is BaseApplication baseApplication)
                    window.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

                window.ViewModel = viewmodel;

                window.PrimaryButtonCommand = viewmodel.SubmitCommand;
                window.SecondaryButtonCommand = viewmodel.CloseCommand;
                window.CloseButtonCommand = viewmodel.CloseCommand;

                window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];

                if (!viewmodel.IsInitialized)
                    await viewmodel.InitializeAsync();

                await OpenDialogAsync(window);
            }
        }

        private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
        {
            if (_activeDialog is not null)
                await CloseDialogAsync(_activeDialog);

            _activeDialog = dialog;

            return await _activeDialog.ShowAsync().AsTask();
        }

        public override Task CloseDialogAsync(IWindow dialog)
        {
            _activeDialog?.Close();
            _activeDialog = null;
            return Task.CompletedTask;
        }
    }
}
