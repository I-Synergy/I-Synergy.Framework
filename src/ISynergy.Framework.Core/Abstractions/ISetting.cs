namespace ISynergy.Framework.Core.Abstractions
{
    /// <summary>
    /// Interface ISetting
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is first run.
        /// </summary>
        /// <value><c>true</c> if this instance is first run; otherwise, <c>false</c>.</value>
        bool IsFirstRun { get; set; }
        /// <summary>
        /// Gets or sets the default decimals.
        /// </summary>
        /// <value>The default decimals.</value>
        int DefaultDecimals { get; set; }
        /// <summary>
        /// Gets or sets the default currency identifier.
        /// </summary>
        /// <value>The default currency identifier.</value>
        int DefaultCurrencyId { get; set; }
        /// <summary>
        /// Gets or sets the default country identifier.
        /// </summary>
        /// <value>The default country identifier.</value>
        int DefaultCountryId { get; set; }
    }
}
