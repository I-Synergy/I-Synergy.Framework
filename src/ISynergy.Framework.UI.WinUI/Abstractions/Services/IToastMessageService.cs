namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface IToastMessageService
{
    void ShowError(string message);
    void ShowInformation(string message);
    void ShowSuccess(string message);
    void ShowWarning(string message);
}