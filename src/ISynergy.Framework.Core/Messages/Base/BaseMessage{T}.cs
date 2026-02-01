namespace ISynergy.Framework.Core.Messages.Base;

/// <summary>
/// Passes a generic value (Content) to a recipient.
/// </summary>
/// <typeparam name="T">The type of the Content property.</typeparam>
public abstract class BaseMessage<T> : BaseMessage
{
    /// <summary>
    /// Initializes a new instance of the GenericMessage class.
    /// </summary>
    /// <param name="content">The message content.</param>
    protected BaseMessage(T content)
    {
        Content = content;
    }

    /// <summary>
    /// Initializes a new instance of the GenericMessage class.
    /// </summary>
    /// <param name="sender">The message's sender.</param>
    /// <param name="content">The message content.</param>
    protected BaseMessage(object sender, T content)
        : base(sender)
    {
        Content = content;
    }

    /// <summary>
    /// Initializes a new instance of the GenericMessage class.
    /// </summary>
    /// <param name="sender">The message's sender.</param>
    /// <param name="target">The message's intended target. This parameter can be used
    /// to give an indication as to whom the message was intended for. Of course
    /// this is only an indication, amd may be null.</param>
    /// <param name="content">The message content.</param>
    protected BaseMessage(object sender, object target, T content)
        : base(sender, target)
    {
        Content = content;
    }

    /// <summary>
    /// Gets or sets the message's content.
    /// </summary>
    public T Content
    {
        get;
        private set;
    }
}
