using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ISynergy.Framework.UI.ViewModels;

/// <summary>
/// View model for the language selection dialog.
/// </summary>
public class LanguageViewModel : ViewModelDialog<CultureInfo>, ILanguageViewModel
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    public override string Title => _commonServices.LanguageService.GetString("Language");

    /// <summary>
    /// Gets the list of supported cultures available for selection.
    /// </summary>
    public IReadOnlyList<LanguageOption> SupportedCultures { get; }

    private List<LanguageOption>? _subscribedOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="supportedCulturesService">The supported cultures service.</param>
    /// <param name="logger">The logger.</param>
    public LanguageViewModel(
        ICommonServices commonServices,
        ISettingsService settingsService,
        ISupportedCulturesService supportedCulturesService,
        ILogger<LanguageViewModel> logger)
        : base(commonServices, logger)
    {
        var cultures = supportedCulturesService.SupportedCultures;
        SupportedCultures = cultures.Select(c => new LanguageOption(c)).ToList().AsReadOnly();

        // Pre-select the saved culture, or fall back to default
        var savedCultureName = settingsService.LocalSettings.Language;
        CultureInfo selectedCulture;

        if (!string.IsNullOrEmpty(savedCultureName)
            && supportedCulturesService.IsCultureSupported(savedCultureName))
        {
            selectedCulture = CultureInfo.GetCultureInfo(savedCultureName);
        }
        else
        {
            selectedCulture = supportedCulturesService.DefaultCulture;
        }

        SetSelectedItem(selectedCulture);

        // Wire up selection events for WinUI/WPF compatibility; store options so
        // we can unsubscribe in Cleanup to avoid the memory leak (P4.1).
        _subscribedOptions = new List<LanguageOption>(SupportedCultures.Count);
        foreach (var option in SupportedCultures)
        {
            option.Selected += OnLanguageOptionSelected;
            _subscribedOptions.Add(option);
        }

        // Mark the initially selected option
        var current = SupportedCultures.FirstOrDefault(c =>
            c.CultureInfo.TwoLetterISOLanguageName.Equals(
                selectedCulture.TwoLetterISOLanguageName,
                StringComparison.OrdinalIgnoreCase));
        if (current is not null)
            current.IsSelected = true;
    }

    private void OnLanguageOptionSelected(object? s, EventArgs _)
    {
        if (s is LanguageOption selected)
            SetSelectedItem(selected.CultureInfo);
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;

            if (_subscribedOptions is not null)
            {
                foreach (var option in _subscribedOptions)
                    option.Selected -= OnLanguageOptionSelected;
                _subscribedOptions = null;
            }

            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
    }
}
