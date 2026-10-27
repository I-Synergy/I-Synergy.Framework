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
    /// <param name="assembly">Unused for discovery (kept for source compatibility); the assembly
    /// is already part of <see cref="AppDomain.CurrentDomain"/> by the time this runs.</param>
    /// <param name="assemblyFilter">An optional filter applied to loaded assembly names.</param>
    /// <remarks>
    /// Discovers candidate assemblies via <see cref="AppDomain.CurrentDomain"/>.GetAssemblies()
    /// rather than walking the static reference graph with Assembly.GetReferencedAssemblies().
    /// The latter throws PlatformNotSupportedException under NativeAOT (no BCL trim/AOT
    /// analyzer warning is emitted for it, so this only surfaces as a runtime crash on
    /// launch) -- see ISynergy.Framework.Core.Extensions.ReflectionExtensions.GetAllReferencedAssemblyNames.
    /// Already-loaded-domain scanning is the same pattern the sibling GetViewModelTypes()/
    /// GetViewTypes()/GetWindowTypes() helpers below use.
    /// </remarks>
    [RequiresUnreferencedCode("Scans loaded assemblies for IView/IWindow/IViewModel implementors; not trim-compatible. Types are only found if the application still references them.")]
    public static void RegisterAssemblies(this IServiceCollection services, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)
    {
        var candidateAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assemblies = new List<Assembly>();

        if (assemblyFilter is not null)
            foreach (var item in candidateAssemblies.Where(a => assemblyFilter(a.GetName())).EnsureNotNull())
                assemblies.Add(item);

        foreach (var item in candidateAssemblies.Where(a =>
            a.GetName().Name!.StartsWith("ISynergy.Framework.UI") ||
            a.GetName().Name!.StartsWith("ISynergy.Framework.Mvvm")).EnsureNotNull())
            if (!assemblies.Contains(item))
                assemblies.Add(item);

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
