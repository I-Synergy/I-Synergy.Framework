using ISynergy.Framework.Core.Enumerations;
using Microsoft.UI.Xaml;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Abstractions.Services
{
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
        /// Sets the style.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="theme">The theme.</param>
        void SetStyle(string color, Themes theme);

        /// <summary>
        /// Ininitialize main window for service.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        void InitializeMainWindow(object mainWindow);

        /// <summary>
        /// Setups the titlebar.
        /// </summary>
        void SetTitlebar();
    }
}
