using System;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Models.Mandate
{
    /// <summary>
    /// Class MandateRequest.
    /// </summary>
    public class MandateRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandateRequest" /> class.
        /// </summary>
        public MandateRequest()
        {
            Method = PaymentMethods.DirectDebit;
        }

        /// <summary>
        /// Payment method of the mandate.
        /// </summary>
        /// <value>The method.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethods Method { get; set; }

        /// <summary>
        /// Required - Name of consumer you add to the mandate
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Required - Consumer's IBAN account
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// Optional - The consumer's bank's BIC / SWIFT code.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }

        /// <summary>
        /// Optional - The date when the mandate was signed.
        /// </summary>
        /// <value>The signature date.</value>
        public DateTime? SignatureDate { get; set; }

        /// <summary>
        /// Optional - A custom reference
        /// </summary>
        /// <value>The mandate reference.</value>
        public string MandateReference { get; set; }
    }
}
