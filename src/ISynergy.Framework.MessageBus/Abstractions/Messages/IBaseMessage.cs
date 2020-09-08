namespace ISynergy.Framework.MessageBus.Abstractions
{
    /// <summary>
    /// Interface IBusMessage
    /// </summary>
    public interface IBaseMessage
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        string Tag { get; set; }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        string ContentType { get; set; }
    }
}
