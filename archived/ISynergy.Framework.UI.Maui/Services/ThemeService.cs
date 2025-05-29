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
        Microsoft.Maui.Controls.Application.AccentColor = Color.FromArgb(Style.Color);

        if (Microsoft.Maui.Controls.Application.Current is Application application)
        {
            if (ResourceUtility.FindResource<Color>(Primary) is Color color)
                Microsoft.Maui.Controls.Application.AccentColor = color;

            if (application.Resources.ContainsKey(Primary))
                application.Resources[Primary] = Microsoft.Maui.Controls.Application.AccentColor;
            else
                application.Resources.Add(Primary, Microsoft.Maui.Controls.Application.AccentColor);

            if (application.Resources.ContainsKey(Secondary))
                application.Resources[Secondary] = Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(0.25f);
            else
                application.Resources.Add(Secondary, Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(0.25f));

            if (application.Resources.ContainsKey(Tertiary))
                application.Resources[Tertiary] = Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(-0.25f);
            else
                application.Resources.Add(Tertiary, Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(-0.25f));

#if ANDROID
            if (application.Resources.ContainsKey("colorPrimary"))
                application.Resources["colorPrimary"] = Microsoft.Maui.Controls.Application.AccentColor;
            else
                application.Resources.Add("colorPrimary", Microsoft.Maui.Controls.Application.AccentColor);

            if (application.Resources.ContainsKey("colorAccent"))
                application.Resources["colorAccent"] = Microsoft.Maui.Controls.Application.AccentColor;
            else
                application.Resources.Add("colorAccent", Microsoft.Maui.Controls.Application.AccentColor);

            if (application.Resources.ContainsKey("colorPrimaryDark"))
                application.Resources["colorPrimaryDark"] = Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(-0.25f);
            else
                application.Resources.Add("colorPrimaryDark", Microsoft.Maui.Controls.Application.AccentColor.AddLuminosity(-0.25f));
#endif

#if WINDOWS
            if (application.Resources.ContainsKey("SystemAccentColor"))
                application.Resources["SystemAccentColor"] = Microsoft.Maui.Controls.Application.AccentColor;
            else
                application.Resources.Add("SystemAccentColor", Microsoft.Maui.Controls.Application.AccentColor);

            if (application.Resources.ContainsKey("SystemColorControlAccentColor"))
                application.Resources["SystemColorControlAccentColor"] = Microsoft.Maui.Controls.Application.AccentColor;
            else
                application.Resources.Add("SystemColorControlAccentColor", Microsoft.Maui.Controls.Application.AccentColor);
#endif
        }

        MessageService.Default.Send(new StyleChangedMessage(Style));
    }
}
