using System.ComponentModel;

namespace ISynergy.Framework.Globalization.Services;

/// <summary>
/// Singleton service that holds localized strings and notifies bindings when language is refreshed.
/// Provides an indexer for binding to localized text without requiring GetString markup.
/// Works with XAML bindings (WPF, WinUI), property binding (Blazor), and data binding (MAUI).
/// </summary>
public sealed class LocalizedStrings : INotifyPropertyChanged
{
    private static readonly object _creationLock = new object();
    private static LocalizedStrings? _defaultInstance;

    /// <summary>
    /// Gets the default static instance of <see cref="LocalizedStrings"/>.
    /// </summary>
    public static LocalizedStrings Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                    {
                        _defaultInstance = new LocalizedStrings();
                    }
                }
            }

            return _defaultInstance;
        }
    }

    /// <summary>
    /// Occurs when a property value changes. Raised with "Item[]" when culture is refreshed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the localized strings are refreshed (typically after a culture change).
    /// Supports UI frameworks (like Blazor) that rely on events rather than INotifyPropertyChanged.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets a localized string by key. Used with XAML bindings like
    /// {Binding [SettingKey], Source={x:Static local:LocalizedStrings.Default}}
    /// or with property binding in Blazor and MAUI.
    /// </summary>
    /// <param name="key">The resource string key.</param>
    /// <returns>The localized string, or [key] if not found.</returns>
    public string this[string key] => LanguageService.Default.GetString(key);

    /// <summary>
    /// Refreshes all bindings by raising PropertyChanged for the indexer (Item[])
    /// and the Changed event. Call this after changing the culture to update all bound UI text.
    /// </summary>
    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
