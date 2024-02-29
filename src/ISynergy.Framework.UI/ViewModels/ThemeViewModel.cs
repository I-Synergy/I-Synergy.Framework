using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
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
    public override string Title => BaseCommonServices.LanguageService.GetString("Theme");


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
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="applicationSettingsService"></param>
    /// <param name="logger">The logger factory.</param>
    public ThemeViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        IBaseApplicationSettingsService applicationSettingsService,
        ILogger logger)
        : base(context, commonServices, logger)
    {
        applicationSettingsService.LoadSettings();

        ThemeColors = new ThemeColors();

        SelectedItem = new Style()
        {
            Color = applicationSettingsService.Settings.Color, 
            Theme = applicationSettingsService.Settings.Theme 
        };
    }
}
