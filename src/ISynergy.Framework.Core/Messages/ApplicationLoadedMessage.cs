using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Message published when the application has completed initialization AND UI is ready.
/// </summary>
public sealed class ApplicationLoadedMessage : BaseMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="ApplicationLoadedMessage"/>.
    /// </summary>
    public ApplicationLoadedMessage()
        : base()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationLoadedMessage"/> with a sender.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    public ApplicationLoadedMessage(object sender)
        : base(sender)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationLoadedMessage"/> with a sender and intended target.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    /// <param name="target">Intended target of the message (may be null).</param>
    public ApplicationLoadedMessage(object sender, object target)
        : base(sender, target)
    {
    }
}
