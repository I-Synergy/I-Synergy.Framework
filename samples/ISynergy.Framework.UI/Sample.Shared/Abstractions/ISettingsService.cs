namespace Sample.Abstractions.Services
{
    /// <summary>
    /// Interface ISettingsService
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets a value indicating whether this instance is first run.
        /// </summary>
        /// <value><c>true</c> if this instance is first run; otherwise, <c>false</c>.</value>
        bool IsFirstRun { get; }
        /// <summary>
        /// Gets the default currency identifier.
        /// </summary>
        /// <value>The default currency identifier.</value>
        int DefaultCurrencyId { get; }
    }
}
