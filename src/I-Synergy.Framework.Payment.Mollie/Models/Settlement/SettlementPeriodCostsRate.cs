namespace ISynergy.Framework.Payment.Mollie.Models.Settlement
{
    /// <summary>
    /// Class SettlementPeriodCostsRate.
    /// </summary>
    public class SettlementPeriodCostsRate
    {
        /// <summary>
        /// An amount object describing the fixed costs.
        /// </summary>
        /// <value>The fixed.</value>
		public Amount Fixed { get; set; }

        /// <summary>
        /// A string describing the variable costs as a percentage.
        /// </summary>
        /// <value>The variable.</value>
		public string Variable { get; set; }
    }
}
