﻿using System;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Chargeback
{
    /// <summary>
    /// Class ChargebackResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class ChargebackResponse : IResponseObject
    {
        /// <summary>
        /// The chargeback's unique identifier, for example chb_n9z0tp.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The amount charged back.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// This optional field will contain the amount that will be deducted from your account, converted to the currency
        /// your account is settled in. It follows the same syntax as the amount property.
        /// </summary>
        /// <value>The settlement amount.</value>
        public Amount SettlementAmount { get; set; }

        /// <summary>
        /// The date and time the chargeback was issued, in ISO 8601 format.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time the chargeback was reversed, in ISO 8601 format.
        /// </summary>
        /// <value>The reversed at.</value>
        public DateTime? ReversedAt { get; set; }

        /// <summary>
        /// The id of the payment this chargeback belongs to.
        /// </summary>
        /// <value>The payment identifier.</value>
		public string PaymentId { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the chargeback. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public ChargebackResponseLinks Links { get; set; }
    }
}
