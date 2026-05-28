using System.Diagnostics.CodeAnalysis;

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
    /// <param name="resourceType">
    /// The resource type whose full name is used to locate the manifest resource stream.
    /// The type and its satellite assemblies must be preserved by the linker in AOT-published applications.
    /// Pass a compile-time <c>typeof(…)</c> expression and suppress IL2026 at the call site, or refactor to pass a
    /// pre-built <see cref="System.Resources.ResourceManager"/> instance.
    /// </param>
    [RequiresUnreferencedCode("ResourceManager requires the resource type and its satellite assemblies to be preserved by the linker.")]
    void AddResourceManager(Type resourceType);
}
