using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
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
        Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "");

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        Task<MessageBoxResult> ShowErrorAsync(string message, string title = "");

        /// <summary>
        /// Shows the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        Task<MessageBoxResult> ShowInformationAsync(string message, string title = "");

        /// <summary>
        /// Shows the warning asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        Task<MessageBoxResult> ShowWarningAsync(string message, string title = "");

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
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK);
    }
}
