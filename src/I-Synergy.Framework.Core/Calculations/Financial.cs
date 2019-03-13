namespace ISynergy.Calculations
{
    public static class Financial
    {
        public static decimal CalcPercAmountOfAmount(decimal previousAmount, decimal actualAmount)
        {
            if(previousAmount != 0)
            {
                return (actualAmount - previousAmount) / (previousAmount / 100) / 100;
            }
            else
            {
                if (actualAmount != 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static decimal CalcAmountOfPercentage(decimal amount, decimal percentage)
        {
            return amount / 100 * percentage;
        }

        public static decimal CalcMarginPercentage(decimal sales_price, decimal purchase_price)
        {
            if (purchase_price != 0)
            {
                return (sales_price - purchase_price) / (purchase_price / 100) /100;
            }
            else
            {
                return 1;
            }
        }
    }
}