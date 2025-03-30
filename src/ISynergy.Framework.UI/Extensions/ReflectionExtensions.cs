using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions;

public static class ReflectionExtensions
{
    /// <summary>
    /// Get viewmodel types from assemblies.
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> ToViewModelTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetTypes())
                if (type.GetInterface(nameof(IViewModel), false) is not null
                    && (type.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(type.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    && type.Name != GenericConstants.ViewModel
                    && !type.IsAbstract
                    && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get viewmodel types from assemblies.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetViewModelTypes() =>
    AppDomain.CurrentDomain.GetAssemblies()
        .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.GetInterface(nameof(IViewModel), false) is not null
            && (type.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(type.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            && type.Name != GenericConstants.ViewModel
            && !type.IsAbstract
            && !type.IsInterface);

    /// <summary>
    /// Get view types from assemblies.
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> ToViewTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetTypes())
                if (Array.Exists(type.GetInterfaces(), a => a != null && a.FullName != null && a.FullName.Equals(typeof(IView).FullName))
                && (type.Name.EndsWith(GenericConstants.View) || type.Name.EndsWith(GenericConstants.Page))
                && type.Name != GenericConstants.View
                && type.Name != GenericConstants.Page
                && !type.IsAbstract
                && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get view types from assemblies.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetViewTypes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetInterfaces()
                .Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IView).FullName))
                && (type.Name.EndsWith(GenericConstants.View) || type.Name.EndsWith(GenericConstants.Page))
                && type.Name != GenericConstants.View
                && type.Name != GenericConstants.Page
                && !type.IsAbstract
                && !type.IsInterface);

    /// <summary>
    /// Get window types from assemblies.
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> ToWindowTypes(this IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (var type in assembly.GetTypes())
                if (Array.Exists(type.GetInterfaces(), a => a != null && a.FullName != null && a.FullName.Equals(typeof(IWindow).FullName))
                && type.Name.EndsWith(GenericConstants.Window)
                && type.Name != GenericConstants.Window
                && !type.IsAbstract
                && !type.IsInterface)
                    yield return type;
    }

    /// <summary>
    /// Get window types from assemblies.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetWindowTypes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(e => !e.FullName!.Contains("Microsoft") && !e.IsDynamic)
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => Array.Exists(type.GetInterfaces(), a => a != null && a.FullName != null && a.FullName.Equals(typeof(IWindow).FullName))
                && type.Name.EndsWith(GenericConstants.Window)
                && type.Name != GenericConstants.Window
                && !type.IsAbstract
                && !type.IsInterface);

    /// <summary>
    /// Register windows.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="windows"></param>
    public static void RegisterWindows(this IServiceCollection services, IEnumerable<Type> windows)
    {
        foreach (var window in windows.Distinct().EnsureNotNull())
            services.RegisterWindow(window);
    }

    /// <summary>
    /// Register window.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="window"></param>
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
    /// <param name="services"></param>
    /// <param name="views"></param>
    public static void RegisterViews(this IServiceCollection services, IEnumerable<Type> views)
    {
        foreach (var view in views.Distinct().EnsureNotNull())
            services.RegisterView(view);
    }

    /// <summary>
    /// Register view.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="view"></param>
    public static void RegisterView(this IServiceCollection services, Type view)
    {
        var abstraction = Array.Find(
            view.GetInterfaces(),
            q => q.GetInterfaces().Contains(typeof(IView)) && !q.Name.StartsWith(nameof(IView)));

        services.Register(view, abstraction);
    }

    /// <summary>
    /// Register windows.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="viewmodels"></param>
    public static void RegisterViewModels(this IServiceCollection services, IEnumerable<Type> viewmodels)
    {
        foreach (var viewmodel in viewmodels.Distinct().EnsureNotNull())
            services.RegisterViewModel(viewmodel);
    }

    /// <summary>
    /// Register window.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="viewmodel"></param>
    public static void RegisterViewModel(this IServiceCollection services, Type viewmodel)
    {
        var abstraction = Array.Find(
            viewmodel.GetInterfaces(),
            q => q.GetInterfaces().Contains(typeof(IViewModel)) && !q.Name.StartsWith(nameof(IViewModel)));

        services.Register(viewmodel, abstraction);
    }

    private static void Register(this IServiceCollection services, Type type, Type? abstraction)
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
