using GalaSoft.MvvmLight.Messaging;
using ISynergy.Mvvm;

namespace ISynergy.Events
{
    public abstract class EventMessage : MessageBase
    {
        public bool Handled { get; set; }

        protected EventMessage(object sender)
            : base(sender)
        {
        }
    }

    public abstract class QueryMessage : EventMessage
    {
        public string Query { get; }

        protected QueryMessage(object sender, string query)
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

    public class OnLanguageChangedMessage : EventMessage
    {
        public string Language { get; }

        public OnLanguageChangedMessage(object sender, string language)
            :base(sender)
        {
            Language = language;
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
        public object Property { get; }
        public bool IsAuthenticated { get; }

        public AuthenticateUserMessageResult(object sender, object property, bool authenticated)
            : base(sender)
        {
            Property = property;
            IsAuthenticated = authenticated;
        }
    }

    public class ItemSelectedMessage : EventMessage
    {
        public object Value { get; }

        public ItemSelectedMessage(object sender, object value)
            : base(sender)
        {
            Value = value;
        }
    }

    public class AddBladeMessage : EventMessage
    {
        public IViewModelBlade Viewmodel { get; }

        public AddBladeMessage(object sender, IViewModelBlade viewmodel)
            : base(sender)
        {
            Viewmodel = viewmodel;
        }
    }

    public class OnSubmitMessage : EventMessage
    {
        public object Value { get; }

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
        public string TileName { get; }

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

    public class OfflineModeChangedMessage : EventMessage
    {
        public OfflineModeChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}
