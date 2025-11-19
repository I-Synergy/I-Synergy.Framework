using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels;

/// <summary>
/// Represents the view model that manages application theme and accent color selection.
/// </summary>
/// <remarks>
/// Initializes its state from <see cref="ISettingsService"/> obtained through the scoped context service.
/// The selected color name is normalized to lowercase to match entries in <see cref="ThemeColors.Colors"/>.
/// Logging is used to trace initialization and to warn if a stored color is not available.
/// </remarks>
public class ThemeViewModel : ViewModelDialog<ThemeStyle>
{
    /// <inheritdoc/>
    public override string Title => _commonServices.LanguageService.GetString("Theme");

    /// <summary>
    /// Gets or sets the available theme colors.
    /// </summary>
    /// <value>
    /// A palette of accent colors available to the application. The default is a new instance of <see cref="ThemeColors"/>.
    /// </value>
    public ThemeColors ThemeColors
    {
        get => GetValue<ThemeColors>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">An application-wide services container that provides access to shared services.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    /// <remarks>
    /// The constructor loads the persisted theme and color from <see cref="ISettingsService"/>, validates that the color exists
    /// in <see cref="ThemeColors.Colors"/>, and falls back to <see cref="ThemeColors.Default"/> when necessary.
    /// </remarks>
    public ThemeViewModel(ICommonServices commonServices, ILogger<ThemeViewModel> logger)
        : base(commonServices, logger)
    {
        ThemeColors = new ThemeColors();
    }
}
