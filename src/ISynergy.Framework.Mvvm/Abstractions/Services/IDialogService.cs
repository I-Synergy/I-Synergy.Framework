using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    Task<MessageBoxResult> ShowErrorAsync(Exception error, string? title = "");

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    Task<MessageBoxResult> ShowErrorAsync(string message, string? title = "");

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    Task<MessageBoxResult> ShowInformationAsync(string message, string? title = "");

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    Task<MessageBoxResult> ShowWarningAsync(string message, string? title = "");

    /// <summary>
    /// Shows the greeting asynchronous.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Task.</returns>
    Task ShowGreetingAsync(string name);

    /// <summary>
    /// Shows message box asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="notificationTypes"></param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    Task<MessageBoxResult> ShowMessageAsync(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default);

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
    /// Shows a dialog with an initial entity and lifecycle callbacks.
    /// The DialogService manages scope creation, ViewModel/Window resolution,
    /// event subscription, callback error handling, and scope disposal.
    /// </summary>
    /// <typeparam name="TWindow">The window type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="e">The entity to edit or pre-populate.</param>
    /// <param name="submitCallback">Invoked when the user submits, with the resulting entity. Runs before scope disposal.</param>
    /// <param name="cancelCallback">Optional. Invoked when the user cancels (CancelCommand / tap-outside). Runs before scope disposal.</param>
    /// <param name="closedCallback">Optional. Invoked when the dialog closes on ANY path (submit, cancel, back button, platform dismiss). Runs before scope disposal.</param>
    Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        TEntity e,
        Func<TEntity, Task> submitCallback,
        Func<Task>? cancelCallback = null,
        Func<Task>? closedCallback = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>;

    /// <summary>
    /// Shows a dialog with lifecycle callbacks (no initial entity — the ViewModel
    /// sets its own initial state via its constructor).
    /// The DialogService manages scope creation, ViewModel/Window resolution,
    /// event subscription, callback error handling, and scope disposal.
    /// </summary>
    /// <typeparam name="TWindow">The window type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="submitCallback">Invoked when the user submits, with the resulting entity. Runs before scope disposal.</param>
    /// <param name="cancelCallback">Optional. Invoked when the user cancels (CancelCommand / tap-outside). Runs before scope disposal.</param>
    /// <param name="closedCallback">Optional. Invoked when the dialog closes on ANY path (submit, cancel, back button, platform dismiss). Runs before scope disposal.</param>
    Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        Func<TEntity, Task> submitCallback,
        Func<Task>? cancelCallback = null,
        Func<Task>? closedCallback = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>;
}
