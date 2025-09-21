using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.UI.Abstractions.Services;

/// <summary>
/// Interface IThemeSelectorService
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    bool IsLightThemeEnabled { get; }

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    Style Style { get; }

    /// <summary>
    /// Sets the theme.
    /// </summary>
    /// <param name="style">The theme.</param>
    void SetStyle(Style style);

    /// <summary>
    /// Ininitialize main window for service.
    /// </summary>
    /// <param name="mainWindow"></param>
    void InitializeMainWindow(object mainWindow);
}
