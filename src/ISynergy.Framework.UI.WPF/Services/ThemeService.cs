using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public class ThemeService : IThemeService
    {
        private readonly IBaseApplicationSettingsService _applicationSettingsService;

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public Style Style
        {
            get => new()
            {
                Theme = _applicationSettingsService.Settings.Theme,
                Color = _applicationSettingsService.Settings.Color
            };
        }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        public bool IsLightThemeEnabled => Style.Theme == Themes.Light;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="applicationSettingsService"></param>
        public ThemeService(IBaseApplicationSettingsService applicationSettingsService)
        {
            _applicationSettingsService = applicationSettingsService;
            _applicationSettingsService.LoadSettings();
        }

        /// <summary>
        /// Sets the theme.
        /// </summary>
        public void SetStyle()
        {
            //Application.AccentColor = Color.FromArgb(Style.Color);

            //if (IsLightThemeEnabled)
            //    Application.Current.Resources.ApplyLightTheme();
            //else
            //    Application.Current.Resources.ApplyDarkTheme();
        }
    }
}
