using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class PayPalPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class PayPalPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentRequest" /> class.
        /// </summary>
        public PayPalPaymentRequest()
        {
            Method = PaymentMethods.PayPal;
        }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        /// <value>The shipping address.</value>
        public AddressObject ShippingAddress { get; set; }
    }
}
