using ISynergy.Framework.Payment.Mollie.Models.List;

using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IPaymentClient
    /// </summary>
    public interface IPaymentClient
    {
        /// <summary>
        /// Creates the payment asynchronous.
        /// </summary>
        /// <param name="paymentRequest">The payment request.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest paymentRequest);

        /// <summary>
        /// Retrieve a single payment object by its payment identifier.
        /// </summary>
        /// <param name="paymentId">The payment's ID, for example tr_7UhSN1zuXS.</param>
        /// <param name="testmode">Oauth - Optional – Set this to true to get a payment made in test mode. If you omit this parameter, you can only retrieve live mode payments.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        Task<PaymentResponse> GetPaymentAsync(string paymentId, bool testmode = false);

        /// <summary>
        /// Some payment methods are cancellable for an amount of time, usually until the next day. Or as long as the payment status is open. Payments may be cancelled manually from the Dashboard, or automatically by using this endpoint.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <returns>Task.</returns>
	    Task DeletePaymentAsync(string paymentId);

        /// <summary>
        /// Retrieve all payments created with the current payment profile, ordered from newest to oldest.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testMode">if set to <c>true</c> [test mode].</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
		Task<ListResponse<PaymentResponse>> GetPaymentListAsync(string from = null, int? limit = null, string profileId = null, bool? testMode = null);

        /// <summary>
        /// Gets the payment list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentResponse>> GetPaymentListAsync(UrlObjectLink<ListResponse<PaymentResponse>> url);
        /// <summary>
        /// Gets the payment asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        Task<PaymentResponse> GetPaymentAsync(UrlObjectLink<PaymentResponse> url);
    }
}
