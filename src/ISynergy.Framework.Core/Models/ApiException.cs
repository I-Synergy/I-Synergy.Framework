namespace ISynergy.Framework.Core.Models
{
    /// <summary>
    /// Class ApiException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException" /> class.
        /// </summary>
        public ApiException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public ApiException(string message, string type)
            : base(message)
        {
            Type = type;
        }
    }
}
