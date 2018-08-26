namespace ISynergy.Common.Calculations
{
    public class Tax
    {
        public static decimal CalcPriceExclVAT(decimal percentage, decimal amountVatIncluded)
        {
            return (amountVatIncluded / (100 + percentage)) * 100;
        }

        public static decimal CalcPriceInclVAT(decimal percentage, decimal amountVatExcluded)
        {
            return amountVatExcluded + ((amountVatExcluded / 100) * percentage);
        }

        public static decimal CalcVATInclVAT(decimal percentage, decimal amountVatIncluded)
        {
            return (amountVatIncluded / (100 + percentage)) * percentage;
        }

        public static decimal CalcVATExclVAT(decimal percentage, decimal amountVatExcluded)
        {
            return (amountVatExcluded / 100) * percentage;
        }
    }
}