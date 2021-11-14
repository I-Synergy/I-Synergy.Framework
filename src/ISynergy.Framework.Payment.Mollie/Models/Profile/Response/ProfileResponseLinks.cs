using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.PaymentMethod;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Profile.Response
{
    /// <summary>
    /// Class ProfileResponseLinks.
    /// </summary>
    public class ProfileResponseLinks
    {
        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<ProfileResponse> Self { get; set; }
        /// <summary>
        /// Gets or sets the chargebacks.
        /// </summary>
        /// <value>The chargebacks.</value>
        public UrlObjectLink<ListResponse<ChargebackResponse>> Chargebacks { get; set; }
        /// <summary>
        /// Gets or sets the methods.
        /// </summary>
        /// <value>The methods.</value>
        public UrlObjectLink<ListResponse<PaymentResponse>> Methods { get; set; }
        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>The payments.</value>
        public UrlObjectLink<ListResponse<PaymentMethodResponse>> Payments { get; set; }
        /// <summary>
        /// Gets or sets the refunds.
        /// </summary>
        /// <value>The refunds.</value>
        public UrlObjectLink<ListResponse<RefundResponse>> Refunds { get; set; }
        /// <summary>
        /// Gets or sets the checkout preview URL.
        /// </summary>
        /// <value>The checkout preview URL.</value>
        public UrlLink CheckoutPreviewUrl { get; set; }
        /// <summary>
        /// Gets or sets the documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
