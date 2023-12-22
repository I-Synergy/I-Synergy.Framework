using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using IThemeService = ISynergy.Framework.Mvvm.Abstractions.Services.IThemeService;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class NavigationService.
/// Implements the <see cref="INavigationService" />
/// </summary>
/// <seealso cref="INavigationService" />
public class NavigationService : INavigationService
{
    private readonly IContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly IThemeService _themeService;

    public event EventHandler BackStackChanged;

    /// <summary>
    /// Handles the <see cref="E:BackStackChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnBackStackChanged(EventArgs e) => BackStackChanged?.Invoke(this, e);

    /// <summary>
    /// The frame.
    /// </summary>
    private Frame _frame;

    /// <summary>
    /// Navigation backstack.
    /// </summary>
    private Stack<object> _backStack = new Stack<object>();

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
    public bool CanGoBack => _backStack.Count > 0 ? true : false;

    /// <summary>
    /// Gets a value indicating whether this instance can go forward.
    /// </summary>
    /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
    public bool CanGoForward => _frame.CanGoForward;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="themeService"></param>
    public NavigationService(
        IContext context,
        IServiceProvider serviceProvider,
        IThemeService themeService)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _themeService = themeService;
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        if (CanGoBack && _backStack.Pop() is IViewModel viewModel)
        {
            await NavigateAsync(viewModel, navigateBack: true);
        }
    }

    /// <summary>
    /// Goes the forward.
    /// </summary>
    public Task GoForwardAsync()
    {
        if (_frame.CanGoForward)
            _frame.GoForward();

        return Task.CompletedTask;
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

        if (_serviceProvider.GetRequiredService(page) is ISynergy.Framework.UI.Controls.View view)
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

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="navigateBack"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel =>
        NavigateAsync(default(TViewModel), parameter);

    /// <summary>
    /// navigate as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="navigateBack"></param>
    /// <returns>Task&lt;IView&gt;.</returns>
    /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
    public Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(viewModel, parameter) is View page)
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(
                DispatcherQueuePriority.Normal,
                async () =>
                {
                    switch (_themeService.Style.Theme)
                    {
                        case Core.Enumerations.Themes.Light:
                            _frame.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
                            break;
                        case Core.Enumerations.Themes.Dark:
                            _frame.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark;
                            break;
                        default:
                            _frame.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Default;
                            break;
                    }

                    // Check if actual page is the same as destination page.
                    if (_frame.Content is View originalView)
                    {
                        if (originalView.GetType().Equals(page.GetType()))
                            return;

                        if (!navigateBack)
                            _backStack.Push(originalView.ViewModel);
                    }

                    _frame.Content = page;

                    if (!page.ViewModel.IsInitialized)
                        await page.ViewModel.InitializeAsync();

                    OnBackStackChanged(EventArgs.Empty);
                });
        }

        return Task.CompletedTask;
    }

    public Task NavigateModalAsync<TViewModel>(object parameter = null) where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(parameter) is View page && Application.Current is BaseApplication baseApplication)
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(
                DispatcherQueuePriority.Normal,
                async () =>
                {
                    switch (_themeService.Style.Theme)
                    {
                        case Core.Enumerations.Themes.Light:
                            page.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
                            break;
                        case Core.Enumerations.Themes.Dark:
                            page.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark;
                            break;
                        default:
                            page.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Default;
                            break;
                    }

                    baseApplication.MainWindow.Content = page;

                    if (!page.ViewModel.IsInitialized)
                        await page.ViewModel.InitializeAsync();
                });
        }

        return Task.CompletedTask;
    }

    public Task CleanBackStackAsync()
    {
        _backStack.Clear();
        OnBackStackChanged(EventArgs.Empty);
        return Task.CompletedTask;
    }
}
