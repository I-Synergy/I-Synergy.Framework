using ISynergy.Framework.Core.Localization;
using ISynergy.Framework.Globalization.Abstractions.Services;
using System.Globalization;
using System.Resources;

namespace ISynergy.Framework.Globalization.Services;

/// <summary>
/// Class LanguageService.
/// </summary>
public sealed class LanguageService : ILanguageService
{
    private static readonly object _creationLock = new object();
    private readonly List<ResourceManager> _managers;
    private static ILanguageService? _defaultInstance;

    /// <summary>
    /// Gets the LanguageService's default instance.
    /// </summary>
    public static ILanguageService Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                    {
                        _defaultInstance = new LanguageService();
                    }
                }
            }

            return _defaultInstance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    public LanguageService()
    {
        _managers = [new ResourceManager(typeof(Properties.Resources))];

        // Register as the ambient language service so Core (which cannot reference this
        // Globalization type) can localize its own messages.
        LanguageServiceProvider.Default = this;
    }

    /// <summary>
    /// Adds the resource manager.
    /// </summary>
    /// <param name="resourceType">
    /// The resource type whose full name is used to locate the manifest resource stream.
    /// The type and its satellite assemblies must be preserved by the linker in AOT-published applications.
    /// </param>
    public void AddResourceManager(Type resourceType) =>
        _managers.Add(new ResourceManager(resourceType));

    /// <summary>
    /// Gets the string by searching resource managers in reverse registration order.
    /// The last-registered resource manager (typically the consuming application's)
    /// is checked first, allowing it to override framework resource strings.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public string GetString(string key)
    {
        // Search in reverse order — last-registered (consumer) wins over framework defaults
        for (int i = _managers.Count - 1; i >= 0; i--)
        {
            string? result = _managers[i].GetString(key, CultureInfo.CurrentUICulture);

            if (!string.IsNullOrEmpty(result))
                return result;
        }

        return $"[{key}]";
    }
}
