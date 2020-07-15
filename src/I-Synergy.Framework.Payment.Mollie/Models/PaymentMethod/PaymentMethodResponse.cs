using System.Collections.Generic;
using ISynergy.Framework.Payment.Mollie.Models.Issuer;
using ISynergy.Framework.Payment.Mollie.Models.PaymentMethod.Pricing;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod
{
    /// <summary>
    /// Class PaymentMethodResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class PaymentMethodResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a method object. Will always contain method for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The unique identifier of the payment method. When used during payment creation, the payment method selection screen will be skipped.
        /// </summary>
        /// <value>The identifier.</value>
        public PaymentMethods Id { get; set; }

        /// <summary>
        /// The full name of the payment method.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// URLs of images representing the payment method.
        /// </summary>
        /// <value>The image.</value>
        public PaymentMethodResponseImage Image { get; set; }

        /// <summary>
        /// List of Issuers
        /// </summary>
        /// <value>The issuers.</value>
        public List<IssuerResponse> Issuers { get; set; }

        /// <summary>
        /// Pricing set of the payment method what will be include if you add the parameter.
        /// </summary>
        /// <value>The pricing.</value>
        public List<PricingResponse> Pricing { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the payment method. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public PaymentMethodResponseLinks Links { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
