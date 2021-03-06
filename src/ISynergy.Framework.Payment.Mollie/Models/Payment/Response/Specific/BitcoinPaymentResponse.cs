﻿namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class BitcoinPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class BitcoinPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public BitcoinPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class BitcoinPaymentResponseDetails.
    /// </summary>
    public class BitcoinPaymentResponseDetails
    {
        /// <summary>
        /// The bitcoin address the bitcoins were transferred to.
        /// </summary>
        /// <value>The bitcoin address.</value>
        public string BitcoinAddress { get; set; }

        /// <summary>
        /// The amount transferred in BTC.
        /// </summary>
        /// <value>The bitcoin amount.</value>
        public Amount BitcoinAmount { get; set; }

        /// <summary>
        /// A URI that is understood by Bitcoin wallet clients and will cause such clients to prepare the transaction.
        /// </summary>
        /// <value>The bitcoin URI.</value>
        public string BitcoinUri { get; set; }

        /// <summary>
        /// Only available when explicitly included. – A QR code that can be scanned by Bitcoin wallet clients and will cause
        /// such clients to prepare the transaction.
        /// </summary>
        /// <value>The qr code.</value>
        public QrCode QrCode { get; set; }
    }
}
