using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels;

/// <summary>
/// Class ThemeViewModel.
/// </summary>
public class ThemeViewModel : ViewModelDialog<Style>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title => LanguageService.Default.GetString("Theme");


    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    public ThemeColors ThemeColors
    {
        get => GetValue<ThemeColors>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    public ThemeViewModel(
        ICommonServices commonServices,
        ILogger logger)
        : base(commonServices, logger)
    {
        ThemeColors = new ThemeColors();

        var settingsService = _commonServices.ScopedContextService.GetService<ISettingsService>();
        SelectedItem = new Style(settingsService.LocalSettings.Color, settingsService.LocalSettings.Theme);
    }
}
