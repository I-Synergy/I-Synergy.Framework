using System;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Models.Mandate
{
    /// <summary>
    /// Class MandateResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class MandateResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a mandate object. Will always contain mandate for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// Unique identifier of you mandate.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Current status of mandate.
        /// </summary>
        /// <value>The status.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public MandateStatus Status { get; set; }

        /// <summary>
        /// Payment method of the mandate. Possible values: directdebit creditcard
        /// </summary>
        /// <value>The method.</value>
        public PaymentMethods Method { get; set; }

        /// <summary>
        /// Mandate details that are different per payment method. Available fields depend on that payment method.
        /// </summary>
        /// <value>The details.</value>
        public MandateDetails Details { get; set; }

        /// <summary>
        /// The mandate’s custom reference, if this was provided when creating the mandate.
        /// </summary>
        /// <value>The mandate reference.</value>
        public string MandateReference { get; set; }

        /// <summary>
        /// The signature date of the mandate in YYYY-MM-DD format.
        /// </summary>
        /// <value>The signature date.</value>
        public string SignatureDate { get; set; }

        /// <summary>
        /// DateTime when mandate was created.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the mandate. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public MandateResponseLinks Links { get; set; }
    }
}
