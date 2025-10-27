using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Message that carries the current profile when authentication changes.
/// </summary>
public sealed class AuthenticationChangedMessage : BaseMessage<IProfile?>
{
    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationChangedMessage"/> with content.
    /// </summary>
    /// <param name="profile">The current profile or null when signed out.</param>
    public AuthenticationChangedMessage(IProfile? profile)
        : base(profile)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationChangedMessage"/> with a sender and content.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    /// <param name="profile">The current profile or null when signed out.</param>
    public AuthenticationChangedMessage(object sender, IProfile? profile)
        : base(sender, profile)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationChangedMessage"/> with a sender, target and content.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    /// <param name="target">Intended target of the message.</param>
    /// <param name="profile">The current profile or null when signed out.</param>
    public AuthenticationChangedMessage(object sender, object target, IProfile? profile)
        : base(sender, target, profile)
    {
    }
}
