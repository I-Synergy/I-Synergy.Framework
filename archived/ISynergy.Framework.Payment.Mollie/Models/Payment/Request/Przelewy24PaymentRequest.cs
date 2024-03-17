using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class Przelewy24PaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class Przelewy24PaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Przelewy24PaymentRequest" /> class.
        /// </summary>
        public Przelewy24PaymentRequest()
        {
            Method = PaymentMethods.Przelewy24;
        }

        /// <summary>
        /// Consumer’s email address, this is required for Przelewy24 payments.
        /// </summary>
        /// <value>The billing email.</value>
        public string BillingEmail { get; set; }
    }
}
