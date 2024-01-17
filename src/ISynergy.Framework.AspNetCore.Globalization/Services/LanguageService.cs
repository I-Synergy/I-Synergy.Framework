namespace ISynergy.Framework.AspNetCore.Globalization.Services;

/// <summary>
/// Class LanguageService.
/// Implements the <see cref="ILanguageService" />
/// </summary>
/// <seealso cref="ILanguageService" />
internal class LanguageService : ILanguageService
{
    /// <summary>
    /// The managers
    /// </summary>
    private readonly List<ResourceManager> _managers;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService" /> class.
    /// </summary>
    public LanguageService()
    {
        _managers = new List<ResourceManager>();
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
        foreach (var manager in _managers)
        {
            var result = manager.GetString(key, CultureInfo.CurrentCulture);

            if (string.IsNullOrEmpty(result))
                result = manager.GetString(key);

            if (!string.IsNullOrEmpty(result))
                return result;
        }

        return $"[{key}]";
    }
}
