using ISynergy.Framework.Core.Abstractions.Services;
using System.Globalization;
using System.Resources;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class _languageService.
/// </summary>
public class LanguageService : ILanguageService
{
    private static readonly object _creationLock = new object();
    private static ILanguageService _defaultInstance;

    /// <summary>
    /// The managers
    /// </summary>
    private readonly List<ResourceManager> _managers;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    public LanguageService()
    {
        _managers = new List<ResourceManager>
        {
            new ResourceManager(typeof(ISynergy.Framework.Core.Properties.Resources))
        };
    }

    /// <summary>
    /// Gets the Messenger's default instance, allowing
    /// to register and send messages in a static manner.
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
            string result = manager.GetString(key, CultureInfo.CurrentCulture);
            if (!string.IsNullOrEmpty(result))
                return result;
        }

        return $"[{key}]";
    }
}
