namespace ISynergy.Framework.Core.Messages.Base;

/// <summary>
/// Base class for all messages broadcasted by the Messenger.
/// You can create your own message types by extending this class.
/// </summary>
public abstract class BaseMessage
{
    /// <summary>
    /// Initializes a new instance of the MessageBase class.
    /// </summary>
    protected BaseMessage()
    {
        Sender = null!;
        Target = null!;
    }

    /// <summary>
    /// Initializes a new instance of the MessageBase class.
    /// </summary>
    /// <param name="sender">The message's original sender.</param>
    protected BaseMessage(object sender)
    {
        Sender = sender;
        Target = null!;
    }

    /// <summary>
    /// Initializes a new instance of the MessageBase class.
    /// </summary>
    /// <param name="sender">The message's original sender.</param>
    /// <param name="target">The message's intended target. This parameter can be used
    /// to give an indication as to whom the message was intended for. Of course
    /// this is only an indication, amd may be null.</param>
    protected BaseMessage(object sender, object target)
        : this(sender)
    {
        Target = target;
    }

    /// <summary>
    /// Gets or sets the message's sender.
    /// </summary>
    public object Sender
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the message's intended target. This property can be used
    /// to give an indication as to whom the message was intended for. Of course
    /// this is only an indication, amd may be null.
    /// </summary>
    public object Target
    {
        get;
        private set;
    }
}
