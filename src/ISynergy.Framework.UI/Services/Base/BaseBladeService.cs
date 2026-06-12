using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services.Base;

/// <summary>
/// Base class for blade services. Handles scope creation, callback wiring,
/// blade lifecycle, and scope disposal. Platform-specific subclasses provide
/// view type resolution.
/// </summary>
public abstract class BaseBladeService : IBladeService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IExceptionHandlerService _exceptionHandlerService;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBladeService"/> class.
    /// </summary>
    protected BaseBladeService(
        IServiceScopeFactory scopeFactory,
        IExceptionHandlerService exceptionHandlerService,
        ILogger logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _exceptionHandlerService = exceptionHandlerService ?? throw new ArgumentNullException(nameof(exceptionHandlerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task OpenBladeAsync<TBladeViewModel, TEntity>(
        IViewModelBladeView owner,
        TEntity entity,
        Func<TEntity, Task>? submittedEventHandler = null,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TBladeViewModel>? initialization = null)
        where TBladeViewModel : IViewModelBlade<TEntity>
    {
        await OpenBladeInternalAsync(owner, entity, submittedEventHandler, cancelledEventHandler, closedEventHandler, initialization);
    }

    /// <inheritdoc />
    public async Task OpenBladeAsync<TBladeViewModel, TEntity>(
        IViewModelBladeView owner,
        Func<TEntity, Task>? submittedEventHandler = null,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TBladeViewModel>? initialization = null)
        where TBladeViewModel : IViewModelBlade<TEntity>
    {
        await OpenBladeInternalAsync<TBladeViewModel, TEntity>(owner, default, submittedEventHandler, cancelledEventHandler, closedEventHandler, initialization);
    }

    private async Task OpenBladeInternalAsync<TBladeViewModel, TEntity>(
        IViewModelBladeView owner,
        TEntity? entity,
        Func<TEntity, Task>? submittedEventHandler,
        Func<Task>? cancelledEventHandler,
        Func<Task>? closedEventHandler,
        Action<TBladeViewModel>? initialization)
        where TBladeViewModel : IViewModelBlade<TEntity>
    {
        var scope = _scopeFactory.CreateScope();
        try
        {
            if (scope.ServiceProvider.GetRequiredService(typeof(TBladeViewModel)) is not IViewModelBlade<TEntity> bladeVm)
            {
                scope.Dispose();
                return;
            }

            // Let caller configure the ViewModel (SetItems, etc.)
            initialization?.Invoke((TBladeViewModel)bladeVm);

            // Set the entity if provided
            if (entity is not null)
                bladeVm.SetSelectedItem(entity);

            // Wire up callbacks
            if (submittedEventHandler is not null)
            {
                var submittedVmTypeName = typeof(TBladeViewModel).Name;
                bladeVm.Submitted += (_, args) =>
                    _ = HandleBladeSubmittedAsync(args.Result, submittedEventHandler, submittedVmTypeName).SafeFireAndForget();
            }

            if (cancelledEventHandler is not null)
            {
                var cancelledVmTypeName = typeof(TBladeViewModel).Name;
                bladeVm.Cancelled += (_, _) =>
                    _ = HandleBladeCancelledAsync(cancelledEventHandler, cancelledVmTypeName).SafeFireAndForget();
            }

            // Scope cleanup when blade closes
            bladeVm.Closed += (_, _) =>
            {
                if (closedEventHandler is not null)
                {
                    var closedVmTypeName = typeof(TBladeViewModel).Name;
                    _ = HandleBladeClosedAsync(closedEventHandler, closedVmTypeName).SafeFireAndForget();
                }

                scope.Dispose();
            };

            // Platform-specific view resolution and blade setup.
            // The scope's ServiceProvider is passed so the view can be resolved
            // from the SAME scope as the ViewModel.
            await SetupAndOpenBladeAsync(owner, bladeVm, scope.ServiceProvider);
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Platform-specific blade setup: resolves the view from <paramref name="serviceProvider"/>
    /// (same scope as the ViewModel), monitors load, initializes the ViewModel,
    /// and adds the blade to the owner.
    /// </summary>
    protected abstract Task SetupAndOpenBladeAsync(IViewModelBladeView owner, IViewModelBlade bladeVm, IServiceProvider serviceProvider);

    /// <summary>
    /// Platform-specific view load monitoring. Default implementation waits for
    /// the view's Loaded event with a timeout.
    /// </summary>
    protected abstract Task MonitorViewLoadAsync(IView view, int timeoutMs = 500);

    /// <inheritdoc />
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        if (viewmodel is not IViewModelBlade bladeVm)
            return;

        SafeOnNavigatedFrom(viewmodel);

        if (owner.Blades is not null)
        {
            var bladeToRemove = owner.Blades
                .FirstOrDefault(q => q.ViewModel == bladeVm && ((IViewModelBlade)q.ViewModel).Owner == bladeVm.Owner);

            if (bladeToRemove is not null && owner.Blades.Remove(bladeToRemove))
            {
                if (owner.Blades.Count < 1)
                    owner.IsPaneVisible = false;
                else if (owner.Blades[^1] is { } blade)
                {
                    blade.IsEnabled = true;
                    if (blade.ViewModel is IViewModel activeVm)
                        SafeOnNavigatedTo(activeVm);
                }
            }
        }
    }

    /// <summary>
    /// Handles the blade Submitted event. Invokes the caller's submitted callback
    /// and logs any exception.
    /// </summary>
    private async Task HandleBladeSubmittedAsync<TEntity>(
        TEntity result,
        Func<TEntity, Task> submittedEventHandler,
        string viewModelTypeName)
    {
        try
        {
            await submittedEventHandler(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blade submit callback failed for {ViewModelType}", viewModelTypeName);
        }
    }

    /// <summary>
    /// Handles the blade Cancelled event. Invokes the caller's cancelled callback
    /// and logs any exception.
    /// </summary>
    private async Task HandleBladeCancelledAsync(
        Func<Task> cancelledEventHandler,
        string viewModelTypeName)
    {
        try
        {
            await cancelledEventHandler();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blade cancel callback failed for {ViewModelType}", viewModelTypeName);
        }
    }

    /// <summary>
    /// Handles the blade Closed event. Invokes the caller's closed callback
    /// and logs any exception. The scope has already been disposed by the
    /// synchronous <see cref="OpenBladeInternalAsync"/> handler.
    /// </summary>
    private async Task HandleBladeClosedAsync(
        Func<Task> closedEventHandler,
        string viewModelTypeName)
    {
        try
        {
            await closedEventHandler();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blade close callback failed for {ViewModelType}", viewModelTypeName);
        }
    }

    /// <summary>
    /// Prepares the owner for a new blade: cancels running commands, sets the blade owner,
    /// and wires up the <see cref="IViewModel.Closed"/> event for auto-removal.
    /// </summary>
    protected void PrepareOwnerForBlade(IViewModelBladeView owner, IViewModelBlade bladeVm)
    {
        if (owner is IViewModel ownerVm)
        {
            ownerVm.CancelAllCommands();
            SafeOnNavigatedFrom(ownerVm);
        }

        bladeVm.Owner = owner;

        // Auto-remove blade when its ViewModel fires Closed
        bladeVm.Closed += (sender, _) =>
        {
            if (sender is IViewModelBlade vm)
                RemoveBlade(vm.Owner, vm);
        };
    }

    /// <summary>
    /// Adds a blade view to the owner's blade collection, disabling existing blades.
    /// </summary>
    protected static void AddBladeToOwner(IViewModelBladeView owner, IView view)
    {
        if (owner.Blades is not null && !owner.Blades.Any(a => a.GetType().FullName!.Equals(view.GetType().FullName)))
        {
            foreach (var blade in owner.Blades.EnsureNotNull())
            {
                blade.IsEnabled = false;
            }

            owner.Blades.Add(view);
        }

        owner.IsPaneVisible = true;
    }

    /// <summary>
    /// Safely calls <see cref="IViewModel.OnNavigatedTo"/> with exception handling.
    /// </summary>
    protected void SafeOnNavigatedTo(IViewModel viewModel)
    {
        try { viewModel.OnNavigatedTo(); }
        catch (Exception ex) { _exceptionHandlerService.HandleException(ex); }
    }

    /// <summary>
    /// Safely calls <see cref="IViewModel.OnNavigatedFrom"/> with exception handling.
    /// </summary>
    protected void SafeOnNavigatedFrom(IViewModel viewModel)
    {
        try { viewModel.OnNavigatedFrom(); }
        catch (Exception ex) { _exceptionHandlerService.HandleException(ex); }
    }
}
