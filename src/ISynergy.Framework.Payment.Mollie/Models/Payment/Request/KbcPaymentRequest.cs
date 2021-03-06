﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class KbcPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class KbcPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KbcPaymentRequest" /> class.
        /// </summary>
        public KbcPaymentRequest()
        {
            Method = PaymentMethods.Kbc;
        }

        /// <summary>
        /// The issuer to use for the KBC/CBC payment. These issuers are not dynamically available through the Issuers API,
        /// but can be retrieved by using the issuers include in the Methods API.
        /// </summary>
        /// <value>The issuer.</value>
        public KbcIssuer Issuer { get; set; }
    }
}
