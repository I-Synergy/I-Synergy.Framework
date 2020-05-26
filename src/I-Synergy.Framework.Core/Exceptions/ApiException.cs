namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Class ApiException.
    /// </summary>
    public class ApiException
    {
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public string Error { get; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="description">The description.</param>
        public ApiException(string error, string description)
        {
            Error = error;
            Description = description;
        }
    }
}
