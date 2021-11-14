using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.PaymentMethod;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IPaymentMethodClient
    /// </summary>
    public interface IPaymentMethodClient
    {
        /// <summary>
        /// Gets the payment method asynchronous.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="locale">The locale.</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <returns>Task&lt;PaymentMethodResponse&gt;.</returns>
        Task<PaymentMethodResponse> GetPaymentMethodAsync(PaymentMethods paymentMethod, bool? includeIssuers = null, string locale = null, bool? includePricing = null, string profileId = null, bool? testmode = null);
        /// <summary>
        /// Gets all payment method list asynchronous.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentMethodResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentMethodResponse>> GetAllPaymentMethodListAsync(string locale = null, bool? includeIssuers = null, bool? includePricing = null);
        /// <summary>
        /// Gets the payment method list asynchronous.
        /// </summary>
        /// <param name="sequenceType">Type of the sequence.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <param name="resource">The resource.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentMethodResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentMethodResponse>> GetPaymentMethodListAsync(SequenceType? sequenceType = null, string locale = null, Amount amount = null, bool? includeIssuers = null, bool? includePricing = null, string profileId = null, bool? testmode = null, Resource? resource = null);
        /// <summary>
        /// Gets the payment method asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PaymentMethodResponse&gt;.</returns>
        Task<PaymentMethodResponse> GetPaymentMethodAsync(UrlObjectLink<PaymentMethodResponse> url);
    }
}
