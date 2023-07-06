using ISynergy.Framework.UI.Controls.ToastNotification.Options;

namespace ISynergy.Framework.UI.Abstractions.Services
{
    public interface IToastMessageService
    {
        void ShowError(string message);
        void ShowError(string message, MessageOptions displayOptions);
        void ShowInformation(string message);
        void ShowInformation(string message, MessageOptions displayOptions);
        void ShowSuccess(string message);
        void ShowSuccess(string message, MessageOptions displayOptions);
        void ShowWarning(string message);
        void ShowWarning(string message, MessageOptions displayOptions);
    }
}