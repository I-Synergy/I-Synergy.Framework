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
public static class ReflectionExtensions
{
    /// <summary>
    /// Get viewmodel types from assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>All discovered ViewModel types.</returns>
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
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
    [RequiresUnreferencedCode("Registers types produced by runtime assembly scanning. For trim-safe registration, call RegisterWindow/RegisterView/RegisterViewModel with statically-known (typeof) types instead.")]
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
    public static void RegisterWindow(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type window)
    {
        var abstraction = FindAbstraction(window, typeof(IWindow), nameof(IWindow));

        services.Register(window, abstraction);
    }

    /// <summary>
    /// Register views.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="views">The view types to register.</param>
    [RequiresUnreferencedCode("Registers types produced by runtime assembly scanning. For trim-safe registration, call RegisterWindow/RegisterView/RegisterViewModel with statically-known (typeof) types instead.")]
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
    public static void RegisterView(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type view)
    {
        var abstraction = FindAbstraction(view, typeof(IView), nameof(IView));

        services.Register(view, abstraction);
    }

    /// <summary>
    /// Register view models.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="viewmodels">The viewmodel types to register.</param>
    [RequiresUnreferencedCode("Registers types produced by runtime assembly scanning. For trim-safe registration, call RegisterWindow/RegisterView/RegisterViewModel with statically-known (typeof) types instead.")]
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
    public static void RegisterViewModel(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type viewmodel)
    {
        var abstraction = FindAbstraction(viewmodel, typeof(IViewModel), nameof(IViewModel));

        if (abstraction is not null)
            services.TryAddScoped(abstraction, viewmodel);

        services.TryAddScoped(viewmodel);
    }

    /// <summary>
    /// Register a view model type with an explicit service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="viewmodel">The viewmodel type to register.</param>
    /// <param name="lifetime">The lifetime to use for the type and its abstraction.</param>
    public static void RegisterViewModel(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type viewmodel,
        ServiceLifetime lifetime)
    {
        var abstraction = FindAbstraction(viewmodel, typeof(IViewModel), nameof(IViewModel));

        if (abstraction is not null)
            services.TryAdd(new ServiceDescriptor(abstraction, viewmodel, lifetime));

        services.TryAdd(new ServiceDescriptor(viewmodel, viewmodel, lifetime));
    }

    [UnconditionalSuppressMessage("Trimming", "IL2070:UnrecognizedReflectionPattern",
        Justification = "Interface types returned by GetInterfaces() are preserved together with the implementing type, and the marker interfaces (IView/IWindow/IViewModel) are statically referenced here, so their interface hierarchies survive trimming.")]
    private static Type? FindAbstraction(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type,
        Type marker,
        string markerName) =>
        Array.Find(type.GetInterfaces(), q => q.GetInterfaces().Contains(marker) && !q.Name.StartsWith(markerName));

    private static void Register(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type,
        Type? abstraction)
    {
        if (type.IsSingleton())
        {
            if (abstraction is not null)
                services.TryAddSingleton(abstraction, type);
            services.TryAddSingleton(type);
        }
        else if (type.IsScoped())
        {
            if (abstraction is not null)
                services.TryAddScoped(abstraction, type);
            services.TryAddScoped(type);
        }
        else
        {
            if (abstraction is not null)
                services.TryAddTransient(abstraction, type);
            services.TryAddTransient(type);
        }
    }
}
