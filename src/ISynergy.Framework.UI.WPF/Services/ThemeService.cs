using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private readonly IApplicationSettingsService _applicationSettingsService;

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public Style Style
    {
        get => new(_applicationSettingsService.Settings.Color, _applicationSettingsService.Settings.Theme);
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
    public ThemeService(IApplicationSettingsService applicationSettingsService)
    {
        _applicationSettingsService = applicationSettingsService;
        _applicationSettingsService.LoadSettings();
    }

    /// <summary>
    /// Sets the theme.
    /// </summary>
    public void SetStyle()
    {
        //Application.Primary = Color.FromArgb(Style.Color);

        //if (IsLightThemeEnabled)
        //    Application.Current.Resources.ApplyLightTheme();
        //else
        //    Application.Current.Resources.ApplyDarkTheme();

        // Add custom resourcedictionaries from code.

        //if (Application.Current is BaseApplication application && application.Resources?.MergedDictionaries is ICollection<ResourceDictionary> dictionary)
        //{
        //    var additionalResourceDictionaries = application.GetAdditionalResourceDictionaries();

        //    dictionary.Add(new Generic());

        //    foreach (var item in additionalResourceDictionaries)
        //    {
        //        dictionary.Add(item);
        //    }
        //}

        MessageService.Default.Send(new StyleChangedMessage(Style));
    }
}
