﻿namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class PayPalPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class PayPalPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public PayPalPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class PayPalPaymentResponseDetails.
    /// </summary>
    public class PayPalPaymentResponseDetails
    {
        /// <summary>
        /// The consumer's first and last name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// The consumer's email address.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// PayPal's reference for the transaction, for instance 9AL35361CF606152E.
        /// </summary>
        /// <value>The pay pal reference.</value>
        public string PayPalReference { get; set; }
    }
}
