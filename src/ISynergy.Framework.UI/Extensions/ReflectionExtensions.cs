using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Reflection-based extension helpers for scanning assemblies and registering UI types.
/// </summary>
/// <remarks>
/// All public methods in this class that call <c>GetExportedTypes</c> or <c>AppDomain.CurrentDomain</c>
/// are annotated with <see cref="RequiresUnreferencedCodeAttribute"/> because they walk the full set of
/// exported types at runtime, which is incompatible with NativeAOT trimming.
/// Use the source-generator-emitted <c>AddUITypes()</c> extension method instead.
/// </remarks>
public static class ReflectionExtensions
{
    /// <summary>
    /// Get viewmodel types from assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>All discovered ViewModel types.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> ToViewModelTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetExportedTypes())
                if (type.GetInterfaces().Any(i => i == typeof(IViewModel) || i.GetInterfaces().Contains(typeof(IViewModel)))
                    && (type.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(type.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    && type.Name != GenericConstants.ViewModel
                    && !type.IsAbstract
                    && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get viewmodel types from all loaded assemblies.
    /// </summary>
    /// <returns>All discovered ViewModel types across the application domain.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> GetViewModelTypes() =>
    AppDomain.CurrentDomain.GetAssemblies()
        .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
        .SelectMany(assembly => assembly.GetExportedTypes())
        .Where(type => type.GetInterfaces().Any(i => i == typeof(IViewModel) || i.GetInterfaces().Contains(typeof(IViewModel)))
            && (type.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(type.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            && type.Name != GenericConstants.ViewModel
            && !type.IsAbstract
            && !type.IsInterface);

    /// <summary>
    /// Get view types from assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>All discovered View types.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> ToViewTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetExportedTypes())
                if (Array.Exists(type.GetInterfaces(), a => a is not null && a.FullName is not null && a.FullName.Equals(typeof(IView).FullName))
                && (type.Name.EndsWith(GenericConstants.View) || type.Name.EndsWith(GenericConstants.Page))
                && type.Name != GenericConstants.View
                && type.Name != GenericConstants.Page
                && !type.IsAbstract
                && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get view types from all loaded assemblies.
    /// </summary>
    /// <returns>All discovered View types across the application domain.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> GetViewTypes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => type.GetInterfaces()
                .Any(a => a is not null && a.FullName is not null && a.FullName.Equals(typeof(IView).FullName))
                && (type.Name.EndsWith(GenericConstants.View) || type.Name.EndsWith(GenericConstants.Page))
                && type.Name != GenericConstants.View
                && type.Name != GenericConstants.Page
                && !type.IsAbstract
                && !type.IsInterface);

    /// <summary>
    /// Get window types from assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>All discovered Window types.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> ToWindowTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetExportedTypes())
                if (Array.Exists(type.GetInterfaces(), a => a is not null && a.FullName is not null && a.FullName.Equals(typeof(IWindow).FullName))
                && type.Name.EndsWith(GenericConstants.Window)
                && type.Name != GenericConstants.Window
                && !type.IsAbstract
                && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get window types from all loaded assemblies.
    /// </summary>
    /// <returns>All discovered Window types across the application domain.</returns>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddUITypes() instead.")]
    public static IEnumerable<Type> GetWindowTypes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => Array.Exists(type.GetInterfaces(), a => a is not null && a.FullName is not null && a.FullName.Equals(typeof(IWindow).FullName))
                && type.Name.EndsWith(GenericConstants.Window)
                && type.Name != GenericConstants.Window
                && !type.IsAbstract
                && !type.IsInterface);

    /// <summary>
    /// Register windows.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="windows">The window types to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterWindows(this IServiceCollection services, IEnumerable<Type> windows)
    {
        foreach (var window in windows.Distinct().EnsureNotNull())
            services.RegisterWindow(window);
    }

    /// <summary>
    /// Register a window type.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="window">The window type to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterWindow(this IServiceCollection services, Type window)
    {
        var abstraction = Array.Find(
            window.GetInterfaces(),
            q => q.GetInterfaces().Contains(typeof(IWindow)) && !q.Name.StartsWith(nameof(IWindow)));

        services.Register(window, abstraction);
    }

    /// <summary>
    /// Register views.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="views">The view types to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterViews(this IServiceCollection services, IEnumerable<Type> views)
    {
        foreach (var view in views.Distinct().EnsureNotNull())
            services.RegisterView(view);
    }

    /// <summary>
    /// Register a view type.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="view">The view type to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterView(this IServiceCollection services, Type view)
    {
        var abstraction = Array.Find(
            view.GetInterfaces(),
            q => q.GetInterfaces().Contains(typeof(IView)) && !q.Name.StartsWith(nameof(IView)));

        services.Register(view, abstraction);
    }

    /// <summary>
    /// Register view models.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="viewmodels">The viewmodel types to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterViewModels(this IServiceCollection services, IEnumerable<Type> viewmodels)
    {
        foreach (var viewmodel in viewmodels.Distinct().EnsureNotNull())
            services.RegisterViewModel(viewmodel);
    }

    /// <summary>
    /// Register a view model type.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="viewmodel">The viewmodel type to register.</param>
    [RequiresUnreferencedCode("Runtime type registration via reflection is not AOT-compatible. Use AddUITypes() instead.")]
    public static void RegisterViewModel(this IServiceCollection services, Type viewmodel)
    {
        var abstraction = Array.Find(
            viewmodel.GetInterfaces(),
            q => q.GetInterfaces().Contains(typeof(IViewModel)) && !q.Name.StartsWith(nameof(IViewModel)));

        services.Register(viewmodel, abstraction);
    }

    private static void Register(this IServiceCollection services, Type type, Type? abstraction)
    {
        if (abstraction is not null)
            services.TryAddTransient(abstraction, type);

        services.TryAddTransient(type);
    }
}
