using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class IdealPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class IdealPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdealPaymentRequest" /> class.
        /// </summary>
        public IdealPaymentRequest()
        {
            Method = PaymentMethods.Ideal;
        }

        /// <summary>
        /// (Optional) iDEAL issuer id. The id could for example be ideal_INGBNL2A. The returned paymentUrl will then directly
        /// point to the ING web site.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }
    }
}
