using System;
using System.Runtime.Serialization;
using ISynergy.Framework.Payment.Mollie.Models.Error;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Exceptions
{
    /// <summary>
    /// Class MollieApiException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class MollieApiException : Exception
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public MollieErrorMessage Details { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MollieApiException" /> class.
        /// </summary>
        /// <param name="json">The json.</param>
        public MollieApiException(string json) : base(ParseErrorMessage(json).ToString())
        {
            Details = ParseErrorMessage(json);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MollieApiException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected MollieApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Parses the error message.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>MollieErrorMessage.</returns>
        private static MollieErrorMessage ParseErrorMessage(string json)
        {
            return JsonConvert.DeserializeObject<MollieErrorMessage>(json);
        }
    }
}
