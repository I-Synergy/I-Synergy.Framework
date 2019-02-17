using System.Windows.Input;

namespace ISynergy.Providers
{
    public interface IAuthenticationProvider
    {
        bool CanCommandBeExecuted(ICommand RelayCommand, object commandParameter);

        bool HasAccessToUIElement(object element, object tag, string authorizationTag);
    }
}