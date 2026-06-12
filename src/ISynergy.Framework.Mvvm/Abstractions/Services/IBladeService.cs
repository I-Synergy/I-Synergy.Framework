using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Service for opening and managing blade panels with callback-based lifecycle handling.
/// The BladeService manages scope creation, ViewModel/View resolution, event subscription,
/// callback error handling, and scope disposal — so callers only provide business logic.
/// </summary>
public interface IBladeService
{
    /// <summary>
    /// Opens a blade with an initial entity and lifecycle callbacks.
    /// </summary>
    /// <typeparam name="TBladeViewModel">The blade ViewModel type. Must implement <see cref="IViewModelBlade{TEntity}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="owner">The blade view owner that will host this blade.</param>
    /// <param name="entity">The entity to pre-populate.</param>
    /// <param name="submittedEventHandler">Optional. Invoked when the user submits.</param>
    /// <param name="cancelledEventHandler">Optional. Invoked when the user cancels.</param>
    /// <param name="closedEventHandler">Optional. Invoked when the blade closes on ANY path.</param>
    /// <param name="initialization">Optional. Invoked after ViewModel resolution but before the blade is shown.</param>
    Task OpenBladeAsync<TBladeViewModel, TEntity>(
        IViewModelBladeView owner,
        TEntity entity,
        Func<TEntity, Task>? submittedEventHandler = null,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TBladeViewModel>? initialization = null)
        where TBladeViewModel : IViewModelBlade<TEntity>;

    /// <summary>
    /// Opens a blade with lifecycle callbacks (no initial entity).
    /// </summary>
    /// <typeparam name="TBladeViewModel">The blade ViewModel type. Must implement <see cref="IViewModelBlade{TEntity}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="owner">The blade view owner that will host this blade.</param>
    /// <param name="submittedEventHandler">Optional. Invoked when the user submits.</param>
    /// <param name="cancelledEventHandler">Optional. Invoked when the user cancels.</param>
    /// <param name="closedEventHandler">Optional. Invoked when the blade closes on ANY path.</param>
    /// <param name="initialization">Optional. Invoked after ViewModel resolution but before the blade is shown.</param>
    Task OpenBladeAsync<TBladeViewModel, TEntity>(
        IViewModelBladeView owner,
        Func<TEntity, Task>? submittedEventHandler = null,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TBladeViewModel>? initialization = null)
        where TBladeViewModel : IViewModelBlade<TEntity>;

    /// <summary>
    /// Opens a blade with a pre-resolved ViewModel (legacy overload for ViewModels
    /// that do not implement <see cref="IViewModelBlade{TEntity}"/>).
    /// The caller manages the ViewModel lifecycle; no callbacks or scope are provided.
    /// Prefer the generic overloads when the ViewModel implements <see cref="IViewModelBlade{TEntity}"/>.
    /// </summary>
    /// <param name="owner">The blade view owner.</param>
    /// <param name="viewmodel">The pre-resolved blade ViewModel.</param>
    Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel);

    /// <summary>
    /// Removes a blade from the owner.
    /// </summary>
    /// <param name="owner">The blade view owner.</param>
    /// <param name="viewmodel">The blade ViewModel to remove.</param>
    void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);
}
