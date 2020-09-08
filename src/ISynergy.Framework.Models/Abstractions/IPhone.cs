namespace ISynergy.Framework.Models.Abstractions
{
    /// <summary>
    /// Interface IPhone
    /// </summary>
    public interface IPhone
    {
        /// <summary>
        /// Gets or sets the area code.
        /// </summary>
        /// <value>The area code.</value>
        string AreaCode { get; set; }
        /// <summary>
        /// Gets or sets the subscriber number.
        /// </summary>
        /// <value>The subscriber number.</value>
        string SubscriberNumber { get; set; }
    }
}
