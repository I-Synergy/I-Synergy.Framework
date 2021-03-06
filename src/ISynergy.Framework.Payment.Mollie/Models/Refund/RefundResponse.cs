﻿using System;
using System.Collections.Generic;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models.Order;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Refund
{
    /// <summary>
    /// Class RefundResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class RefundResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a refund object. Will always contain refund for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The refund's unique identifier, for example re_4qqhO89gsT.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The amount refunded to the consumer with this refund.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// This optional field will contain the amount that will be deducted from your account balance, converted
        /// to the currency your account is settled in. It follows the same syntax as the amount property. Note that
        /// for refunds, the value key of settlementAmount will be negative. Any amounts not settled by ISynergy.Framework.Payment.Mollie will
        /// not be reflected in this amount, e.g. PayPal refunds.
        /// </summary>
        /// <value>The settlement amount.</value>
        public Amount SettlementAmount { get; set; }

        /// <summary>
        /// The description of the refund that may be shown to the consumer, depending on the payment method used.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Since refunds may be delayed for certain payment methods, the refund carries a status field.
        /// </summary>
        /// <value>The status.</value>
        public RefundStatus Status { get; set; }

        /// <summary>
        /// An array of order line objects as described in Get order. Only available if the refund was created via the
        /// Create Order Refund API.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<OrderAddressDetails> Lines { get; set; }

        /// <summary>
        /// The date and time the refund was issued, in ISO 8601 format.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The unique identifier of the payment this refund was created for. For example: tr_7UhSN1zuXS. The full
        /// payment object can be retrieved via the payment URL in the _links object.
        /// </summary>
        /// <value>The payment identifier.</value>
        public string PaymentId { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the refund. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public RefundResponseLinks Links { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Id: {Id} - PaymentId: {PaymentId}";
        }
    }
}
