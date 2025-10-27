using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Indicates that the initial UI has been created and is ready (first page loaded).
/// Used to coordinate with application initialization to emit <see cref="ApplicationLoadedMessage"/> exactly once.
/// </summary>
public sealed class ApplicationUiReadyMessage : BaseMessage
{
    /// <summary>
    /// Default ctor.
    /// </summary>
    public ApplicationUiReadyMessage()
    : base()
    {
    }

    /// <summary>
    /// Creates a new instance with sender.
    /// </summary>
    public ApplicationUiReadyMessage(object sender)
    : base(sender)
    {
    }

    /// <summary>
    /// Creates a new instance with sender and target.
    /// </summary>
    public ApplicationUiReadyMessage(object sender, object target)
    : base(sender, target)
    {
    }
}
