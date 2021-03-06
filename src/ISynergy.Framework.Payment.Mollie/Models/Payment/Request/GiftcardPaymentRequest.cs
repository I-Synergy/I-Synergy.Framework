﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class GiftcardPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class GiftcardPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GiftcardPaymentRequest" /> class.
        /// </summary>
        public GiftcardPaymentRequest()
        {
            Method = PaymentMethods.GiftCard;
        }

        /// <summary>
        /// The gift card brand to use for the payment. These issuers are not dynamically available through the Issuers API,
        /// but can be retrieved by using the issuers include in the Methods API. If you need a brand not in the list, contact
        /// our support department. If only one issuer is activated on your account, you can omit this parameter.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// The card number on the gift card.
        /// </summary>
        /// <value>The voucher number.</value>
        public string VoucherNumber { get; set; }

        /// <summary>
        /// The PIN code on the gift card. Only required if there is a PIN code printed on the gift card.
        /// </summary>
        /// <value>The voucher pin.</value>
        public string VoucherPin { get; set; }
    }
}
