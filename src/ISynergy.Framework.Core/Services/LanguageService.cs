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
    /// <param name="resourceType">The resource manager.</param>
    public void AddResourceManager(Type resourceType) =>
        _managers.Add(new ResourceManager(resourceType));

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public string GetString(string key)
    {
        foreach (var manager in _managers.EnsureNotNull())
        {
            string? result = manager.GetString(key, CultureInfo.CurrentCulture);

            if (!string.IsNullOrEmpty(result))
                return result;
        }

        return $"[{key}]";
    }
}
