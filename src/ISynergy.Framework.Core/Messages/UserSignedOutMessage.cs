using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Published by <see cref="ISynergy.Framework.Core.Abstractions.Services.IAuthenticationService"/>
/// when a user signs out or the session expires.
/// </summary>
public sealed class UserSignedOutMessage : BaseMessage
{
    public UserSignedOutMessage()
        : base()
    {
    }
}
