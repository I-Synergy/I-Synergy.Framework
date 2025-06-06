﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;

namespace ISynergy.Framework.UI.ViewModels;

/// <summary>
/// Class LanguageViewModel.
/// Implements the <see cref="ILanguageViewModel" />
/// </summary>
/// <seealso cref="ILanguageViewModel" />
public class LanguageViewModel : ViewModelDialog<Languages>, ILanguageViewModel
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title => LanguageService.Default.GetString("Language");

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    public LanguageViewModel(ICommonServices commonServices)
        : base(commonServices)
    {
        SelectedItem = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Language;
    }
}
