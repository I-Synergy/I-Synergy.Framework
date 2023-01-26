using ISynergy.Framework.Core.Abstractions;

namespace Sample.Abstractions.Services
{
    /// <summary>
    /// Interface ISettingsService
    /// </summary>
    public interface ISettingsService<TSettings>
        where TSettings : class, ISetting
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        TSettings Settings { get; }
        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="cancellationToken"></param>
        Task<int> AddOrUpdateSettingsAsync(TSettings e, CancellationToken cancellationToken = default);
        /// <summary>
        /// Loads the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task GetSettingsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultvalue">The defaultvalue.</param>
        /// <returns>T.</returns>
        T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
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
