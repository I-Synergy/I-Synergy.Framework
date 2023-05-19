using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="INavigationService" />
    /// </summary>
    /// <seealso cref="INavigationService" />
    public class NavigationService : BaseNavigationService
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
        public override object Frame
        {
            get => _frame ??= (Frame)Microsoft.UI.Xaml.Window.Current.Content;
            set => _frame = (Frame)value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public override bool CanGoBack => _frame.CanGoBack;
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public override bool CanGoForward => _frame.CanGoForward;

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
        public override void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public override void GoForward()
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
        public override async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
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
        public override void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
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

        /// <summary>
        /// navigate as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
        public override async Task NavigateAsync<TViewModel>(object parameter = null)
        {
            if (NavigationExtensions.CreatePage<TViewModel>(parameter) is View page)
            {
                // Check if actual page is the same as destination page.
                if (_frame.Content is not null && _frame.Content.GetType().Equals(page))
                    return;

                _frame.Navigate(page.GetType(), page.ViewModel);

                if (!page.ViewModel.IsInitialized)
                    await page.ViewModel.InitializeAsync();
            }
        }

        public override Task NavigateModalAsync<TViewModel>(object parameter = null)
        {
            if (NavigationExtensions.CreatePage<TViewModel>(parameter) is View page && Application.Current is BaseApplication baseApplication)
            {
                DispatcherQueue.GetForCurrentThread().TryEnqueue(
                    DispatcherQueuePriority.Normal, 
                    async () =>
                {
                    baseApplication.MainWindow.Content = page;

                    if (!page.ViewModel.IsInitialized)
                        await page.ViewModel.InitializeAsync();
                });
            }

            return Task.CompletedTask;
        }

        public override Task CleanBackStackAsync()
        {
            _frame.BackStack.Clear();
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
