namespace ISynergy.Framework.Financial;

/// <summary>
/// Calculations regarding Value Added Tax (VAT).
/// </summary>
// S101: the type name is acronym casing for Value Added Tax and is public API; renaming it to
// 'Vat' would break every consumer.
#pragma warning disable S101 // established public API name (acronym)
public static class VAT
{
    /// <summary>
    /// Calculates nett price from amount with VAT included.
    /// </summary>
    /// <param name="vatPercentage">The vat percentage.</param>
    /// <param name="amountVatIncluded">The amount vat included.</param>
    /// <returns>System.double.</returns>
    public static decimal CalculateAmountFromAmountIncludingVAT(decimal vatPercentage, decimal amountVatIncluded)
    {
        return (amountVatIncluded / (100 + vatPercentage)) * 100;
    }

    /// <summary>
    /// Calculates gross price with VAT included from nett amount.
    /// </summary>
    /// <param name="vatPercentage">The vat percentage.</param>
    /// <param name="amountVatExcluded">The amount vat excluded.</param>
    /// <returns>System.double.</returns>
    public static decimal CalculateAmountFromAmountExcludingVAT(decimal vatPercentage, decimal amountVatExcluded)
    {
        return amountVatExcluded + ((amountVatExcluded / 100) * vatPercentage);
    }

    /// <summary>
    /// Calculates nett VAT from amount with VAT included.
    /// </summary>
    /// <param name="vatPercentage">The vat percentage.</param>
    /// <param name="amountVatIncluded">The amount vat included.</param>
    /// <returns>System.double.</returns>
    public static decimal CalculateVATFromAmountIncludingVAT(decimal vatPercentage, decimal amountVatIncluded)
    {
        return (amountVatIncluded / (100 + vatPercentage)) * vatPercentage;
    }

    /// <summary>
    /// Calculates nett VAT from amount where VAT is not included.
    /// </summary>
    /// <param name="vatPercentage">The vat percentage.</param>
    /// <param name="amountVatExcluded">The amount vat excluded.</param>
    /// <returns>System.double.</returns>
    public static decimal CalculateVATFromAmountExcludingVAT(decimal vatPercentage, decimal amountVatExcluded)
    {
        return (amountVatExcluded / 100) * vatPercentage;
    }
}
#pragma warning restore S101
