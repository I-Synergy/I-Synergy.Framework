using ISynergy.Framework.Core.Abstractions.Base;
using System.Runtime;

namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Interface IBaseApplicationSettingsService
/// </summary>
public interface ISettingsService
{
    void ClearSettings();
    /// <summary>
    /// Gets the local settings.
    /// </summary>
    /// <value>The settings.</value>
    ILocalSettings LocalSettings { get; }
    /// <summary>
    /// Gets the roaming settings.
    /// </summary>
    /// <value>The settings.</value>
    IRoamingSettings RoamingSettings { get; }
    /// <summary>
    /// Gets the global settings.
    /// </summary>
    IGlobalSettings GlobalSettings { get; }

    #region "Local Settings"
    /// <summary>
    /// Loads the local settings.
    /// </summary>
    void LoadLocalSettings();
    /// <summary>
    /// Saves the local settings.
    /// </summary>
    void SaveLocalSettings();
    #endregion

    #region "Roaming Settings"
    /// <summary>
    /// Loads the roaming settings.
    /// </summary>
    Task LoadRoamingSettingsAsync();
    /// <summary>
    /// Saves the roaming settings.
    /// </summary>
    Task SaveRoamingSettingsAsync();
    #endregion

    #region "Global Settings"
    /// <summary>
    /// Updates the settings.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken"></param>
    Task<int> AddOrUpdateGlobalSettingsAsync(IGlobalSettings e, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task LoadGlobalSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the setting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    /// <returns>T.</returns>
    T GetGlobalSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
    #endregion
}
