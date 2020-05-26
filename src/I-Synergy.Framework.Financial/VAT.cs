using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Financial
{
    /// <summary>
    /// Calculations regarding Value Added Tax (VAT).
    /// </summary>
    public static class VAT
    {
        /// <summary>
        /// Calculates nett price from amount with VAT included.
        /// </summary>
        /// <param name="vatPercentage"></param>
        /// <param name="amountVatIncluded"></param>
        /// <returns></returns>
        public static decimal CalculateAmountFromAmountExcludingVAT(decimal vatPercentage, decimal amountVatIncluded)
        {
            return amountVatIncluded / (100 + vatPercentage) * 100;
        }

        /// <summary>
        /// Calculates gross price with VAT included from nett amount.
        /// </summary>
        /// <param name="vatPercentage"></param>
        /// <param name="amountVatExcluded"></param>
        /// <returns></returns>
        public static decimal CalculateAmountFromAmountIncludingVAT(decimal vatPercentage, decimal amountVatExcluded)
        {
            return amountVatExcluded + (amountVatExcluded / 100 * vatPercentage);
        }

        /// <summary>
        /// Calculates nett VAT from amount with VAT included.
        /// </summary>
        /// <param name="vatPercentage"></param>
        /// <param name="amountVatIncluded"></param>
        /// <returns></returns>
        public static decimal CalculateVATFromAmountIncludingVAT(decimal vatPercentage, decimal amountVatIncluded)
        {
            return amountVatIncluded / (100 + vatPercentage) * vatPercentage;
        }

        /// <summary>
        /// Calculates nett VAT from amount where VAT is not included.
        /// </summary>
        /// <param name="vatPercentage"></param>
        /// <param name="amountVatExcluded"></param>
        /// <returns></returns>
        public static decimal CalculateVATFromAmountExcludingVAT(decimal vatPercentage, decimal amountVatExcluded)
        {
            return amountVatExcluded / 100 * vatPercentage;
        }
    }
}
