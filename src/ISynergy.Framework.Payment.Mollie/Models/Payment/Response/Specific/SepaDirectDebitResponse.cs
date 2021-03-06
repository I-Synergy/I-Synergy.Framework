﻿namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class SepaDirectDebitResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class SepaDirectDebitResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public SepaDirectDebitResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class SepaDirectDebitResponseDetails.
    /// </summary>
    public class SepaDirectDebitResponseDetails
    {
        /// <summary>
        /// Transfer reference used by ISynergy.Framework.Payment.Mollie to identify this payment.
        /// </summary>
        /// <value>The transfer reference.</value>
        public string TransferReference { get; set; }

        /// <summary>
        /// The creditor identifier indicates who is authorized to execute the payment. In this case, it is a reference to
        /// ISynergy.Framework.Payment.Mollie.
        /// </summary>
        /// <value>The creditor identifier.</value>
        public string CreditorIdentifier { get; set; }

        /// <summary>
        /// Optional – The consumer's name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Optional – The consumer's IBAN.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// Optional – The consumer's bank's BIC.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }


        /// <summary>
        /// Estimated date the payment is debited from the consumer's bank account, in YYYY-MM-DD format.
        /// </summary>
        /// <value>The due date.</value>
        public string DueDate { get; set; }

        /// <summary>
        /// Only available if the payment has been verified – Date the payment has been signed by the consumer, in YYYY-MM-DD format.
        /// format.
        /// </summary>
        /// <value>The signature date.</value>
        public string SignatureDate { get; set; }

        /// <summary>
        /// Only available if the payment has failed – The official reason why this payment has failed. A detailed description
        /// of each reason is available on the website of the European Payments Council.
        /// </summary>
        /// <value>The bank reason code.</value>
        public string BankReasonCode { get; set; }

        /// <summary>
        /// Only available if the payment has failed – A textual desciption of the failure reason.
        /// </summary>
        /// <value>The bank reason.</value>
        public string BankReason { get; set; }

        /// <summary>
        /// Only available for batch transactions – The original end-to-end identifier that you've specified in your batch.
        /// </summary>
        /// <value>The end to end identifier.</value>
        public string EndToEndIdentifier { get; set; }

        /// <summary>
        /// Only available for batch transactions – The original mandate reference that you've specified in your batch.
        /// </summary>
        /// <value>The mandate reference.</value>
        public string MandateReference { get; set; }

        /// <summary>
        /// Only available for batch transactions – The original batch reference that you've specified in your batch.
        /// </summary>
        /// <value>The batch reference.</value>
        public string BatchReference { get; set; }

        /// <summary>
        /// Only available for batch transactions – The original file reference that you've specified in your batch.
        /// </summary>
        /// <value>The file reference.</value>
        public string FileReference { get; set; }

    }
}
