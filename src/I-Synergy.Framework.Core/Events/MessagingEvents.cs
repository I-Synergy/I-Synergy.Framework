using GalaSoft.MvvmLight.Messaging;
using ISynergy.ViewModels.Base;

namespace ISynergy.Events
{
    public abstract class EventMessage : MessageBase
    {
        public bool Handled { get; set; }

        public EventMessage(object sender)
            : base(sender)
        {
        }
    }

    public abstract class QueryMessage : EventMessage
    {
        public string Query { get; }

        public QueryMessage(object sender, string query)
            : base(sender)
        {
            Query = query;
        }
    }

    public class CloseWindowsMessage : EventMessage
    {
        public CloseWindowsMessage(object sender)
            : base(sender)
        {
        }
    }

    public class AuthenticationChangedMessage : EventMessage
    {
        public AuthenticationChangedMessage(object sender)
            : base(sender)
        {
        }
    }

    public class AuthenticateUserMessageRequest : EventMessage
    {
        public object Property { get; }
        public bool EnableLogin { get; }
        public bool ShowLogin { get; }

        public AuthenticateUserMessageRequest(object sender, bool showLogin, object property = null, bool enableLogin = true)
            : base(sender)
        {
            Property = property;
            EnableLogin = enableLogin;
            ShowLogin = showLogin;
        }
    }

    public class AuthenticateUserMessageResult : EventMessage
    {
        public object Property { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public AuthenticateUserMessageResult(object sender, object property, bool authenticated)
            : base(sender)
        {
            Property = property;
            IsAuthenticated = authenticated;
        }
    }

    public class ItemSelectedMessage : EventMessage
    {
        public object Value { get; private set; }

        public ItemSelectedMessage(object sender, object value)
            : base(sender)
        {
            Value = value;
        }
    }

    public class AddBladeMessage : EventMessage
    {
        public IViewModelBlade Viewmodel { get; private set; }

        public AddBladeMessage(object sender, IViewModelBlade viewmodel)
            : base(sender)
        {
            Viewmodel = viewmodel;
        }
    }

    public class OnSubmitMessage : EventMessage
    {
        public object Value { get; private set; }

        public OnSubmitMessage(object sender, object value)
            : base(sender)
        {
            Value = value;
            Handled = false;
        }
    }

    public class OnCancelMessage : EventMessage
    {
        public OnCancelMessage(object sender)
            : base(sender)
        {
            Handled = false;
        }
    }

    public class TileSelectedMessage : EventMessage
    {
        public string TileName { get; private set; }

        public TileSelectedMessage(object sender, string name)
            : base(sender)
        {
            TileName = name;
        }
    }

    public class RefreshSettingsMessage : EventMessage
    {
        public RefreshSettingsMessage(object sender)
            : base(sender)
        {
        }
    }

    public class RefreshDashboarMessage : EventMessage
    {
        public RefreshDashboarMessage(object sender)
            : base(sender)
        {
        }
    }

    public class ExceptionHandledMessage : EventMessage
    {
        public ExceptionHandledMessage(object sender)
            : base(sender)
        {
        }
    }
}
