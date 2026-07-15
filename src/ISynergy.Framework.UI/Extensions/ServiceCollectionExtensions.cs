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
    [RequiresUnreferencedCode("Discovers and loads referenced assemblies at runtime and scans them for IView/IWindow/IViewModel implementors; not trim-compatible. Types are only found if the application still references them.")]
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
    [RequiresUnreferencedCode("Scans the supplied assemblies for IView/IWindow/IViewModel implementors; not trim-compatible.")]
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
