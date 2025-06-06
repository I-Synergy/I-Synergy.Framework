﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;

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
    public ThemeViewModel(ICommonServices commonServices)
        : base(commonServices)
    {
        ThemeColors = new ThemeColors();

        SelectedItem = new Style(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Theme);
    }
}
