using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Message published when application initialization has completed.
/// </summary>
public sealed class ApplicationInitializedMessage : BaseMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="ApplicationInitializedMessage"/>.
    /// </summary>
    public ApplicationInitializedMessage()
        : base()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationInitializedMessage"/> with a sender.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    public ApplicationInitializedMessage(object sender)
        : base(sender)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationInitializedMessage"/> with a sender and intended target.
    /// </summary>
    /// <param name="sender">Original sender of the message.</param>
    /// <param name="target">Intended target of the message (may be null).</param>
    public ApplicationInitializedMessage(object sender, object target)
        : base(sender, target)
    {
    }
}
