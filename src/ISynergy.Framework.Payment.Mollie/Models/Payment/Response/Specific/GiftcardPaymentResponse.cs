﻿using System.Collections.Generic;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class GiftcardPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class GiftcardPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// The voucher number, with the last four digits masked. When multiple gift cards are used, this is the first voucher
        /// number. Example: 606436353088147****.
        /// </summary>
        /// <value>The voucher number.</value>
        public string VoucherNumber { get; set; }

        /// <summary>
        /// A list of details of all giftcards that are used for this payment. Each object will contain the following properties.
        /// </summary>
        /// <value>The giftcards.</value>
        public List<Giftcard> Giftcards { get; set; }

        /// <summary>
        /// Only available if another payment method was used to pay the remainder amount – The amount that was paid with
        /// another payment method for the remainder amount.
        /// </summary>
        /// <value>The remainder amount.</value>
        public Amount RemainderAmount { get; set; }

        /// <summary>
        /// Only available if another payment method was used to pay the remainder amount – The payment method that was used to
        /// pay the remainder amount.
        /// </summary>
        /// <value>The remainder method.</value>
        public string RemainderMethod { get; set; }
    }

    /// <summary>
    /// Class Giftcard.
    /// </summary>
    public class Giftcard
    {
        /// <summary>
        /// The ID of the gift card brand that was used during the payment.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// The amount in EUR that was paid with this gift card.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// The voucher number, with the last four digits masked. Example: 606436353088147****
        /// </summary>
        /// <value>The voucher number.</value>
        public string VoucherNumber { get; set; }
    }
}
