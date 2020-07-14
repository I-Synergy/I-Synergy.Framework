using System;
using ISynergy.Framework.Payment.Mollie.Models.Error;

namespace ISynergy.Framework.Payment.Mollie.Exceptions
{
    /// <summary>
    /// Class MollieApiException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
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
        /// Parses the error message.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>MollieErrorMessage.</returns>
        private static MollieErrorMessage ParseErrorMessage(string json)
        {
            return JsonSerializer.Deserialize<MollieErrorMessage>(json);
        }
    }
}
