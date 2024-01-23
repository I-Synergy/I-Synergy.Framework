using Sample.Models;

namespace Sample.Abstractions.Services;

/// <summary>
/// Interface ISettingsService
/// </summary>
public interface IGlobalSettingsService
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <value>The settings.</value>
    GlobalSettings Settings { get; }
    /// <summary>
    /// Updates the settings.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken"></param>
    Task<int> AddOrUpdateSettingsAsync(GlobalSettings e, CancellationToken cancellationToken = default);
    /// <summary>
    /// Loads the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task LoadSettingsAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Clears the settings.
    /// </summary>
    void ClearSettings();
    /// <summary>
    /// Gets the setting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    /// <returns>T.</returns>
    T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
}
