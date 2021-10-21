using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Class ViewNotRegisteredException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class ViewNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotRegisteredException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no
        /// inner exception is specified.</param>
        public ViewNotRegisteredException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ViewNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
