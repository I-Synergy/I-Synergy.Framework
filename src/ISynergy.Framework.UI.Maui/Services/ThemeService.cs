using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Theme;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public class ThemeService : IThemeService
    {
        private const string AccentColor = nameof(AccentColor);
        private const string AccentColorLight = nameof(AccentColorLight);
        private const string AccentColorDark = nameof(AccentColorDark);

        private readonly IBaseApplicationSettingsService _applicationSettingsService;

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public Style Style { get => new()
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
            Application.AccentColor = Color.FromArgb(Style.Color);

            // Add custom resourcedictionaries from code.
            if (Application.Current is BaseApplication application && application.Resources?.MergedDictionaries is ICollection<ResourceDictionary> dictionary)
            {
                application.Resources.Clear();

                var additionalResourceDictionaries = application.GetAdditionalResourceDictionaries();

                foreach (var item in additionalResourceDictionaries.Reverse())
                {
                    // Accent color.
                    if (item.ContainsKey(AccentColor) && item[AccentColor] is Color accentColor)
                    {
                        Application.AccentColor = accentColor;

                        item.Remove(AccentColor);

                        if (!application.Resources.ContainsKey(nameof(AccentColor)))
                            application.Resources.Add(nameof(AccentColor), accentColor);

                        if (!application.Resources.ContainsKey("colorPrimary"))
                            application.Resources.Add("colorPrimary", accentColor);

                        if (!application.Resources.ContainsKey("colorAccent"))
                            application.Resources.Add("colorAccent", accentColor);
                    }

                    // Accent color light.
                    if (item.ContainsKey(AccentColorLight) && item[AccentColorLight] is Color accentColorLight)
                    {
                        item.Remove(AccentColorLight);

                        if(!application.Resources.ContainsKey(nameof(AccentColorLight)))
                            application.Resources.Add(nameof(AccentColorLight), accentColorLight);
                    }

                    // Accent color dark
                    if (item.ContainsKey(AccentColorDark) && item[AccentColorDark] is Color accentColorDark)
                    {
                        item.Remove(AccentColorDark);

                        if (!application.Resources.ContainsKey(nameof(AccentColorDark)))
                            application.Resources.Add(nameof(AccentColorDark), accentColorDark);

                        if (!application.Resources.ContainsKey("colorPrimaryDark"))
                            application.Resources.Add("colorPrimaryDark", accentColorDark);
                    }
                }

                if (!application.Resources.ContainsKey(nameof(AccentColor)))
                    application.Resources.Add(nameof(AccentColor), Application.AccentColor);

                if (!application.Resources.ContainsKey(nameof(AccentColorLight)))
                    application.Resources.Add(nameof(AccentColorLight), Application.AccentColor.AddLuminosity(0.25f));

                if (!application.Resources.ContainsKey(nameof(AccentColorDark)))
                    application.Resources.Add(nameof(AccentColorDark), Application.AccentColor.AddLuminosity(-0.25f));

                if (!application.Resources.ContainsKey("colorPrimary"))
                    application.Resources.Add("colorPrimary", Application.AccentColor);

                if (!application.Resources.ContainsKey("colorAccent"))
                    application.Resources.Add("colorAccent", Application.AccentColor);

                if (!application.Resources.ContainsKey("colorPrimaryDark"))
                    application.Resources.Add("colorPrimaryDark", Application.AccentColor.AddLuminosity(-0.25f));

                dictionary.Clear();
                dictionary.Add(new Generic());

                foreach (var item in additionalResourceDictionaries)
                {
                    dictionary.Add(item);
                }

                if (IsLightThemeEnabled)
                    application.UserAppTheme = AppTheme.Light;
                else
                    application.UserAppTheme = AppTheme.Dark;
            }
        }
    }
}
