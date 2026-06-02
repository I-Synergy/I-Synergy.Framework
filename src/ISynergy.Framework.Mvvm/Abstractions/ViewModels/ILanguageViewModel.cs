using ISynergy.Framework.Core.Models;
using System.Globalization;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface for the language selection dialog view model.
/// </summary>
public interface ILanguageViewModel : IViewModelDialog<CultureInfo>
{
    /// <summary>
    /// Gets the list of supported cultures available for selection.
    /// </summary>
    IReadOnlyList<LanguageOption> SupportedCultures { get; }
}
