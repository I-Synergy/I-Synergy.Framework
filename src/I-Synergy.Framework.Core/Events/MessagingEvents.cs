using ISynergy.ViewModels.Base;

namespace ISynergy.Events
{
    public class CloseTabMessage
    {
        public object ViewModel { get; set; }
    }

    public class ActivateTabMessage
    {
        public object TabControl { get; set; }
    }

    public class CloseWindowsMessage
    {
        public IViewModel ViewModel { get; set; }
    }

    public class ChangeWallpaperMessage { public object Value { get; set; } }

    public class LoginAuthenticationMessage { }

    public class AuthenticateUserMessageRequest
    {
        public object Property { get; set; }
        public object Sender { get; set; }
        public bool EnableLogin { get; set; }
    }

    public class AuthenticateUserMessageResult
    {
        public object Property { get; set; }
        public bool IsHandled { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class ItemSelectedMessage { public object Value { get; set; } }

    public class AddBladeMessage
    {
        public object Owner { get; internal set; }
        public IViewModelBlade Viewmodel { get; internal set; }

        public AddBladeMessage(object owner, IViewModelBlade viewmodel)
        {
            Owner = owner;
            Viewmodel = viewmodel;
        }
    }

    public abstract class EventMessage
    {
        public bool Handled { get; set; }
        public object Sender { get; internal set; }
    }

    public class OnSubmittanceMessage : EventMessage
    {
        public object Value { get; internal set; }

        public OnSubmittanceMessage(object sender, object value)
        {
            Sender = sender;
            Value = value;
            Handled = false;
        }
    }

    public class OnCancellationMessage : EventMessage
    {
        public OnCancellationMessage(object sender)
        {
            Sender = sender;
            Handled = false;
        }
    }

    public class TileSelectedMessage { public string TileName { get; set; } }

    public class RefreshSettingsMessage { }

    public class RefreshDashboarMessage { }

    public class LoginMessage { }

    public class ExceptionHandledMessage { }
}
