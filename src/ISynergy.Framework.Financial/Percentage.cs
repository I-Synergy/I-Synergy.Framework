namespace ISynergy.Framework.Financial
{
    /// <summary>
    /// Calculations based on percentages.
    /// </summary>
    public static class Percentage
    {
        /// <summary>
        /// Calculates increase/decrease percentage of previous amount compared to actual amount.
        /// </summary>
        /// <param name="previousAmount">The previous amount.</param>
        /// <param name="actualAmount">The actual amount.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal CalculatePercentageAmountOfAmount(decimal previousAmount, decimal actualAmount)
        {
            if (previousAmount > 0 || previousAmount < 0)
            {
                return ((actualAmount - previousAmount) / (previousAmount / 100)) / 100;
            }
            else if(actualAmount == previousAmount)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Calculates value from percentage of amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal CalculateAmountOfPercentage(decimal amount, decimal percentage)
        {
            return amount / 100 * percentage;
        }

        /// <summary>
        /// Calculates margin between sales and purchase price in percent.
        /// </summary>
        /// <param name="salesPrice">The sales price.</param>
        /// <param name="purchasePrice">The purchase price.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal CalculateMarginPercentage(decimal salesPrice, decimal purchasePrice)
        {
            if (purchasePrice > 0 || purchasePrice < 0)
            {
                return ((salesPrice - purchasePrice) / (purchasePrice / 100)) /100;
            }
            else if (salesPrice == purchasePrice)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
