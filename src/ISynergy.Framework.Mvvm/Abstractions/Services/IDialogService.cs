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
    /// Shows a dialog with an initial entity and lifecycle callbacks.
    /// The DialogService manages scope creation, ViewModel/Window resolution,
    /// event subscription, callback error handling, and scope disposal.
    /// </summary>
    /// <typeparam name="TWindow">The window type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="e">The entity to edit or pre-populate.</param>
    /// <param name="submittedEventHandler">Invoked when the user submits, with the resulting entity. Runs before scope disposal.</param>
    /// <param name="cancelledEventHandler">Optional. Invoked when the user cancels (CancelCommand / tap-outside). Runs before scope disposal.</param>
    /// <param name="closedEventHandler">Optional. Invoked when the dialog closes on ANY path (submit, cancel, back button, platform dismiss). Runs before scope disposal.</param>
    /// <param name="initialization">Optional. Invoked after ViewModel resolution but before the dialog is shown. Use to call SetItems, SetSelectedItems, or other ViewModel configuration.</param>
    Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        TEntity e,
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
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
    /// <param name="submittedEventHandler">Invoked when the user submits, with the resulting entity. Runs before scope disposal.</param>
    /// <param name="cancelledEventHandler">Optional. Invoked when the user cancels (CancelCommand / tap-outside). Runs before scope disposal.</param>
    /// <param name="closedEventHandler">Optional. Invoked when the dialog closes on ANY path (submit, cancel, back button, platform dismiss). Runs before scope disposal.</param>
    /// <param name="initialization">Optional. Invoked after ViewModel resolution but before the dialog is shown. Use to call SetItems, SetSelectedItems, or other ViewModel configuration.</param>
    Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>;
}
