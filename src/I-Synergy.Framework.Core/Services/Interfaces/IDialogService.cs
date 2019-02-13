using ISynergy.Enumerations;
using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    /// <summary>
    /// Interface abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in WPF.
    /// </summary>
    public interface IDialogService
    {
        Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "");

        Task<MessageBoxResult> ShowErrorAsync(string message, string title = "");

        Task<MessageBoxResult> ShowInformationAsync(string message, string title = "");

        Task<MessageBoxResult> ShowWarningAsync(string message, string title = "");

        Task ShowGreetingAsync(string name);

        Task<MessageBoxResult> ShowAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information);
    }
}
