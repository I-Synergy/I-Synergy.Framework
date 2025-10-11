using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Utilities;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private const string Primary = nameof(Primary);
    private const string Secondary = nameof(Secondary);
    private const string Tertiary = nameof(Tertiary);

    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public Style Style
    {
        get => new(_settingsService.LocalSettings.Color, _settingsService.LocalSettings.Theme);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => Style.Theme == Themes.Light;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="settingsService"></param>
    public ThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Sets the theme.
    /// </summary>
    public void SetStyle()
    {
        // Set the accent color from settings - this is the primary fix
        Application.AccentColor = Color.FromArgb(Style.Color);

        if (Application.Current is Application application)
        {
            if (ResourceUtility.FindResource<Color>(Primary) is Color color)
                Application.AccentColor = color;

            if (application.Resources.ContainsKey(Primary))
                application.Resources[Primary] = Application.AccentColor;
            else
                application.Resources.Add(Primary, Application.AccentColor);

            if (application.Resources.ContainsKey(Secondary))
                application.Resources[Secondary] = Application.AccentColor.AddLuminosity(0.25f);
            else
                application.Resources.Add(Secondary, Application.AccentColor.AddLuminosity(0.25f));

            if (application.Resources.ContainsKey(Tertiary))
                application.Resources[Tertiary] = Application.AccentColor.AddLuminosity(-0.25f);
            else
                application.Resources.Add(Tertiary, Application.AccentColor.AddLuminosity(-0.25f));

#if ANDROID
            if (application.Resources.ContainsKey("colorPrimary"))
                application.Resources["colorPrimary"] = Application.AccentColor;
            else
                application.Resources.Add("colorPrimary", Application.AccentColor);

            if (application.Resources.ContainsKey("colorAccent"))
                application.Resources["colorAccent"] = Application.AccentColor;
            else
                application.Resources.Add("colorAccent", Application.AccentColor);

            if (application.Resources.ContainsKey("colorPrimaryDark"))
                application.Resources["colorPrimaryDark"] = Application.AccentColor.AddLuminosity(-0.25f);
            else
                application.Resources.Add("colorPrimaryDark", Application.AccentColor.AddLuminosity(-0.25f));
#endif

#if WINDOWS
            if (application.Resources.ContainsKey("SystemAccentColor"))
                application.Resources["SystemAccentColor"] = Application.AccentColor;
            else
                application.Resources.Add("SystemAccentColor", Application.AccentColor);

            if (application.Resources.ContainsKey("SystemColorControlAccentColor"))
                application.Resources["SystemColorControlAccentColor"] = Application.AccentColor;
            else
                application.Resources.Add("SystemColorControlAccentColor", Application.AccentColor);
#endif
        }

        MessengerService.Default.Send(new StyleChangedMessage(Style));
    }
}
