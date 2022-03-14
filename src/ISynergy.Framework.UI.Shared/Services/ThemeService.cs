using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public partial class ThemeService : IThemeService
    {
        private Style _style;
        private ConfigurationOptions _options;

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public Style Style { get => _style; }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        public bool IsLightThemeEnabled => _style.Theme == Themes.Light;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="applicationSettingsService"></param>
        /// <param name="options"></param>
        public ThemeService(
            IBaseApplicationSettingsService applicationSettingsService,
            IOptions<ConfigurationOptions> options)
        {
            _options = options.Value;

            _style = new Style
            {
                Theme = applicationSettingsService.Settings.Theme,
                Color = applicationSettingsService.Settings.Color
            };

            SetStyle(_style);
        }
    }
}
