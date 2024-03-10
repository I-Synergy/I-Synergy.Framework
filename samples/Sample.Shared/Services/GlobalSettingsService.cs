using ISynergy.Framework.Core.Extensions;
using Sample.Abstractions;
using Sample.Models;

namespace Sample.Services;
public class GlobalSettingsService : ISettingsService<GlobalSettings>
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <value>The settings.</value>
    public GlobalSettings Settings { get; private set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public GlobalSettingsService()
    {
    }

    /// <summary>
    /// Updates the settings.
    /// </summary>
    /// <param name="e">The e.</param>
    public void UpdateSettings(GlobalSettings e)
    {
        Settings = e;
    }

    /// <summary>
    /// Gets the setting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    /// <returns>T.</returns>
    public T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>
    {
        return Settings.GetPropertyValue(name, defaultvalue);
    }

    /// <summary>
    /// load settings as an asynchronous operation.
    /// </summary>
    public Task LoadSettingsAsync(CancellationToken cancellationToken = default)
    {
        Settings = new GlobalSettings();
        return Task.CompletedTask;
    }

    public Task<int> AddOrUpdateSettingsAsync(GlobalSettings e, CancellationToken cancellationToken = default)
    {
        // Update settings
        return Task.FromResult(1);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is first run.
    /// </summary>
    /// <value><c>true</c> if this instance is first run; otherwise, <c>false</c>.</value>
    public bool IsFirstRun => Settings.IsFirstRun;

    /// <summary>
    /// Gets the default currency identifier.
    /// </summary>
    /// <value>The default currency identifier.</value>
    public int DefaultCurrencyId => Settings.CurrencyId;
}
