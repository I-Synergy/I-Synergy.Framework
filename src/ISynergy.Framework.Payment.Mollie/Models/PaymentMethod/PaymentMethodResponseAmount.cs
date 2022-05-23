namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod
{
    /// <summary>
    /// Class PaymentMethodResponseAmount.
    /// </summary>
    public class PaymentMethodResponseAmount
    {
        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public decimal Minimum { get; set; }
        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public decimal Maximum { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"Minimum: {Minimum} - Maximum: {Maximum}";
    }
}
