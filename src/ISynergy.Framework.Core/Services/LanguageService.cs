using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using System.Globalization;
using System.Resources;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class LanguageService.
/// </summary>
public sealed class LanguageService : ILanguageService
{
    private readonly List<ResourceManager> _managers;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    public LanguageService()
    {
        _managers = [new ResourceManager(typeof(ISynergy.Framework.Core.Properties.Resources))];
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
