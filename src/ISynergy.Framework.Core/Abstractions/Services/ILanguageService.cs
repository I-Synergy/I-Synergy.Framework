namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Interface ILanguageService
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    string GetString(string key);

    /// <summary>
    /// Adds the resource manager.
    /// </summary>
    /// <param name="resourceType">The resource manager.</param>
    void AddResourceManager(Type resourceType);
}
