using ISynergy.Framework.Core.Enumerations;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
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
        void SetStyle();

        void InitializeMainWindow(object mainWindow);
        void SetTitlebar();
    }
}
