using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Utilities;
using Microsoft.Maui.Controls;

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
        Application.AccentColor = Color.FromArgb(Style.Color);

        if (Application.Current is BaseApplication application)
        {
            if (ResourceUtility.FindResource<Color>(Primary) is Color color)
                Application.AccentColor = color;

            RemoveColors(application.Resources);

            if (application.Resources?.MergedDictionaries is { } resourceDictionaries)
            {
                foreach (var resourceDictionary in resourceDictionaries.EnsureNotNull())
                    RemoveColors(resourceDictionary);
            }

            application.Resources.Add(Primary, Application.AccentColor);
            application.Resources.Add("colorPrimary", Application.AccentColor);
            application.Resources.Add("colorAccent", Application.AccentColor);
            application.Resources.Add("colorPrimaryDark", Application.AccentColor.AddLuminosity(-0.25f));
            application.Resources.Add(nameof(Secondary), Application.AccentColor.AddLuminosity(0.25f));
            application.Resources.Add(nameof(Tertiary), Application.AccentColor.AddLuminosity(-0.25f));

            if (IsLightThemeEnabled)
                application.UserAppTheme = AppTheme.Light;
            else
                application.UserAppTheme = AppTheme.Dark;
        }
    }

    private void RemoveColors(ResourceDictionary resourceDictionary)
    {
        if (resourceDictionary is not null)
        {
            if (ResourceUtility.FindResource<Color>(resourceDictionary, Primary) is not null)
                resourceDictionary.Remove(Primary);

            if (ResourceUtility.FindResource<Color>(resourceDictionary, "colorPrimary") is not null)
                resourceDictionary.Remove("colorPrimary");

            if (ResourceUtility.FindResource<Color>(resourceDictionary, "colorAccent") is not null)
                resourceDictionary.Remove("colorAccent");

            if (ResourceUtility.FindResource<Color>(resourceDictionary, "colorPrimaryDark") is not null)
                resourceDictionary.Remove("colorPrimaryDark");

            if (ResourceUtility.FindResource<Color>(resourceDictionary, Secondary) is not null)
                resourceDictionary.Remove(Secondary);

            if (ResourceUtility.FindResource<Color>(resourceDictionary, Tertiary) is not null)
                resourceDictionary.Remove(Tertiary);
        }
    }
}
