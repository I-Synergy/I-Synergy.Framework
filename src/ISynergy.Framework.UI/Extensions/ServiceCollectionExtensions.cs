using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> for UI type registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the assemblies by scanning them at runtime for <c>IView</c>, <c>IWindow</c>,
    /// and <c>IViewModel</c> implementors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The entry assembly used to resolve referenced assemblies.</param>
    /// <param name="assemblyFilter">An optional filter applied to referenced assembly names.</param>
    /// <remarks>
    /// This overload uses runtime assembly scanning and is not AOT-compatible.
    /// Replace with the source-generator-emitted <c>AddUITypes()</c> extension method for
    /// NativeAOT and trimmed publish scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterAssemblies(this IServiceCollection services, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)
    {
        var referencedAssemblies = assembly.GetAllReferencedAssemblyNames();
        var assemblies = new List<Assembly>();

        if (assemblyFilter is not null)
            foreach (var item in referencedAssemblies.Where(assemblyFilter).EnsureNotNull())
                assemblies.Add(Assembly.Load(item));

        foreach (var item in referencedAssemblies.Where(x =>
            x.Name!.StartsWith("ISynergy.Framework.UI") ||
            x.Name!.StartsWith("ISynergy.Framework.Mvvm")).EnsureNotNull())
            assemblies.Add(Assembly.Load(item));

        services.RegisterAssemblies(assemblies);
    }

    /// <summary>
    /// Registers the assemblies by scanning a set of pre-loaded assemblies for
    /// <c>IView</c>, <c>IWindow</c>, and <c>IViewModel</c> implementors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <remarks>
    /// This overload uses runtime assembly scanning and is not AOT-compatible.
    /// Replace with the source-generator-emitted <c>AddUITypes()</c> extension method for
    /// NativeAOT and trimmed publish scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var viewTypes = assemblies.ToViewTypes();
        var windowTypes = assemblies.ToWindowTypes();
        var viewModelTypes = assemblies.ToViewModelTypes();

        services.RegisterViewModels(viewModelTypes);
        services.RegisterViews(viewTypes);
        services.RegisterWindows(windowTypes);
    }
}
