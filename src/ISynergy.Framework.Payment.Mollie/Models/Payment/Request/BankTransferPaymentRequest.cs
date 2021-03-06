﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class BankTransferPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class BankTransferPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BankTransferPaymentRequest" /> class.
        /// </summary>
        public BankTransferPaymentRequest()
        {
            Method = PaymentMethods.BankTransfer;
        }

        /// <summary>
        /// Optional - Consumer's e-mail address, to automatically send the bank transfer details to. Please note: the payment
        /// instructions will be sent immediately when creating the payment. if you don't specify the locale parameter, the
        /// email will be sent in English, as we haven't yet been able to detect the consumer's browser language.
        /// </summary>
        /// <value>The billing email.</value>
        public string BillingEmail { get; set; }

        /// <summary>
        /// Optional - The date the payment should expire, in YYYY-MM-DD format. Please note: The minimum date is tomorrow and
        /// the maximum date is 100 days.
        /// </summary>
        /// <value>The due date.</value>
        public string DueDate { get; set; }
    }
}
