﻿namespace ISynergy.Framework.Payment.Mollie.Models
{
    /// <summary>
    /// Class Amount.
    /// </summary>
    public class Amount
    {
        /// <summary>
        /// An ISO 4217 currency code. The currencies supported depend on the payment methods that are enabled on your account.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; }

        /// <summary>
        /// An ISO 4217 currency code. The currencies supported depend on the payment methods that are enabled on your account.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Amount" /> class.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="value">The value.</param>
        public Amount(string currency, string value)
        {
            Currency = currency;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Amount" /> class.
        /// </summary>
        public Amount() { }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Value} {Currency}";
        }
    }
}
