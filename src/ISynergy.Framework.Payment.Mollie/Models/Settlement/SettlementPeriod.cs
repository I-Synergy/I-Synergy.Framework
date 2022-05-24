using System.Collections.Generic;

namespace ISynergy.Framework.Payment.Mollie.Models.Settlement
{
    /// <summary>
    /// Class SettlementPeriod.
    /// </summary>
    public class SettlementPeriod
    {
        /// <summary>
        /// The total revenue for each payment method during this period.
        /// </summary>
        /// <value>The revenue.</value>
        public List<SettlementPeriodRevenue> Revenue { get; set; }

        /// <summary>
        /// The fees withheld for each payment method during this period.
        /// </summary>
        /// <value>The costs.</value>
        public List<SettlementPeriodCosts> Costs { get; set; }
    }
}
