﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class CreditCardPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class CreditCardPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardPaymentRequest" /> class.
        /// </summary>
        public CreditCardPaymentRequest()
        {
            Method = PaymentMethods.CreditCard;
        }

        /// <summary>
        /// The card holder’s address details. We advise to provide these details to improve the credit card
        /// fraud protection, and thus improve conversion.
        /// </summary>
        /// <value>The billing address.</value>
        public AddressObject BillingAddress { get; set; }

        /// <summary>
        /// The shipping address details. We advise to provide these details to improve the credit card fraud
        /// protection, and thus improve conversion.
        /// </summary>
        /// <value>The shipping address.</value>
        public AddressObject ShippingAddress { get; set; }
    }
}
