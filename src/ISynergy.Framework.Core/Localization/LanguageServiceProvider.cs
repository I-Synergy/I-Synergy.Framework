using ISynergy.Framework.Globalization.Abstractions.Services;

namespace ISynergy.Framework.Core.Localization;

/// <summary>
/// Ambient access point for the application's <see cref="ILanguageService"/>.
/// </summary>
/// <remarks>
/// <see cref="ISynergy.Framework.Core"/> cannot reference the concrete
/// <c>ISynergy.Framework.Globalization.Services.LanguageService</c> — that type owns the framework
/// resource (satellite) assemblies and therefore lives in the Globalization package. The concrete
/// implementation registers itself here when it is constructed, so Core callers
/// (<c>Argument</c>, <c>LocalizedDescriptionAttribute</c>, <c>BusyService</c>) keep working without a
/// project dependency on Globalization. Until a provider is supplied, a neutral null-object that
/// returns <c>[key]</c> is used.
/// </remarks>
public static class LanguageServiceProvider
{
    private static ILanguageService? _current;

    /// <summary>
    /// Gets or sets the current <see cref="ILanguageService"/>. When no provider has been supplied, a
    /// neutral null-object that returns <c>[key]</c> is returned.
    /// </summary>
    public static ILanguageService Default
    {
        get => _current ?? NullLanguageService.Instance;
        set => _current = value;
    }

    /// <summary>
    /// Gets a value indicating whether a real <see cref="ILanguageService"/> provider has been supplied.
    /// </summary>
    public static bool HasProvider => _current is not null;

    private sealed class NullLanguageService : ILanguageService
    {
        public static readonly NullLanguageService Instance = new();

        public string GetString(string key) => $"[{key}]";

        public void AddResourceManager(Type resourceType) { }
    }
}
