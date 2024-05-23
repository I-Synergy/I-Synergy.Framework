using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Core.Abstractions.Services.Base;

/// <summary>
/// Interface IBaseApplicationSettingsService
/// </summary>
public interface IApplicationSettingsService
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <value>The settings.</value>
    IApplicationSettings Settings { get; }
    /// <summary>
    /// Loads the settings.
    /// </summary>
    void LoadSettings();
    /// <summary>
    /// Saves the settings.
    /// </summary>
    void SaveSettings();
}
