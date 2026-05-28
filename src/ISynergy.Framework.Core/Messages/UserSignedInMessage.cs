using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Published by <see cref="ISynergy.Framework.Core.Abstractions.Services.IAuthenticationService"/>
/// when a user successfully signs in.
/// </summary>
public sealed class UserSignedInMessage : BaseMessage<IProfile>
{
    public UserSignedInMessage(IProfile profile)
        : base(profile)
    {
    }
}
