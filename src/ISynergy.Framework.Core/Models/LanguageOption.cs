using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Display model wrapping <see cref="CultureInfo"/> for use in language selection UI.
/// </summary>
public class LanguageOption : INotifyPropertyChanged
{
    private bool _isSelected;
    private readonly CultureInfo _cultureInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageOption"/> class.
    /// </summary>
    /// <param name="cultureInfo">The underlying culture information.</param>
    public LanguageOption(CultureInfo cultureInfo)
    {
        _cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
    }

    /// <summary>
    /// Gets the underlying <see cref="CultureInfo"/>.
    /// </summary>
    public CultureInfo CultureInfo => _cultureInfo;

    /// <summary>
    /// Gets the native name of the culture (e.g. "Nederlands" for Dutch).
    /// </summary>
    public string NativeName => _cultureInfo.NativeName;

    /// <summary>
    /// Gets the display name with the first letter capitalized for UI consistency.
    /// </summary>
    public string DisplayName =>
        string.IsNullOrEmpty(NativeName) || NativeName.Length == 0
            ? NativeName
            : char.ToUpperInvariant(NativeName[0]) + NativeName[1..];

    /// <summary>
    /// Gets the English name of the culture (e.g. "Dutch" for Dutch).
    /// </summary>
    public string EnglishName => _cultureInfo.EnglishName;

    /// <summary>
    /// Gets the full culture name (e.g. "nl-NL").
    /// </summary>
    public string Name => _cultureInfo.Name;

    /// <summary>
    /// Gets the two-letter ISO language name (e.g. "nl").
    /// </summary>
    public string TwoLetterISOLanguageName => _cultureInfo.TwoLetterISOLanguageName;

    /// <summary>
    /// Gets or sets a value indicating whether this language option is selected.
    /// When set to <c>true</c>, fires the <see cref="Selected"/> event.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            RaisePropertyChanged();

            if (value)
                Selected?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Occurs when this language option becomes selected (<see cref="IsSelected"/> set to <c>true</c>).
    /// </summary>
    public event EventHandler? Selected;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="LanguageOption"/>.
    /// Comparison is based on the <see cref="TwoLetterISOLanguageName"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is LanguageOption other)
            return string.Equals(TwoLetterISOLanguageName, other.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase);

        return false;
    }

    /// <summary>
    /// Returns a hash code for this instance based on <see cref="TwoLetterISOLanguageName"/>.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(TwoLetterISOLanguageName);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The native name of the culture.</returns>
    public override string ToString() => NativeName;
}
