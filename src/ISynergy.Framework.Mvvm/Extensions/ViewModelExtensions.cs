using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Extensions;

/// <summary>
/// Internal registry that maps view type names to their <see cref="Type"/> objects.
/// Populated at application startup via <see cref="ViewModelExtensions.RegisterViewTypes"/>.
/// </summary>
internal static class ViewTypeRegistry
{
    private static IReadOnlyDictionary<string, Type> _registry =
        new Dictionary<string, Type>(StringComparer.Ordinal);

    /// <summary>
    /// Replaces the current registry with the supplied map.
    /// </summary>
    /// <param name="registry">The view-name to <see cref="Type"/> map.</param>
    internal static void Register(IReadOnlyDictionary<string, Type> registry)
        => _registry = registry;

    /// <summary>
    /// Attempts to resolve a view type by name from the registry.
    /// </summary>
    /// <param name="name">The view type name to look up.</param>
    /// <returns>The matching <see cref="Type"/>, or <c>null</c> if not found.</returns>
    internal static Type? TryGet(string name)
        => _registry.TryGetValue(name, out var type) ? type : null;
}

/// <summary>
/// ViewModel extensions.
/// </summary>
public static class ViewModelExtensions
{
    private static Func<Assembly, bool> assemblyFilter =>
        (x =>
        !x.FullName!.StartsWith("System") &&
        !x.FullName!.StartsWith("WinRT") &&
        !x.FullName!.StartsWith("Microsoft") &&
        !x.FullName!.StartsWith("Syncfusion") &&
        !x.FullName!.StartsWith("Snippets") &&
        !x.FullName!.StartsWith("Xamarin") &&
        !x.FullName!.StartsWith("netstandard") &&
        !x.FullName!.StartsWith("mscorlib"));

    /// <summary>
    /// In case of an generic viewmodel, this function returns the base name from IViewModel. 
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    public static string GetViewModelName(this IViewModel viewModel) =>
        viewModel.GetType().GetViewModelName();

    /// <summary>
    /// In case of an generic viewmodel, this function returns the base name from Type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetViewModelName(this Type type)
    {
        var result = type.Name;

        if (type.IsGenericType)
        {
            result = type.GetGenericTypeDefinition().Name.Remove(type.GetGenericTypeDefinition().Name.IndexOf('`'));
        }

        return result;
    }

    /// <summary>
    /// In case of an generic viewmodel, this function returns the base name from IViewModel. 
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    public static string GetViewModelFullName(this IViewModel viewModel) =>
        viewModel.GetType().GetViewModelFullName();

    /// <summary>
    /// In case of an generic viewmodel, this function returns the base name from Type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetViewModelFullName(this Type type)
    {
        if (type.FullName is null)
            throw new NullReferenceException($"{type}: FullName is null");

        var result = type.FullName;

        if (type.IsGenericType && !string.IsNullOrEmpty(type.GetGenericTypeDefinition().FullName))
        {
            result = type.GetGenericTypeDefinition().FullName!.Remove(type.GetGenericTypeDefinition().FullName!.IndexOf('`'));
        }

        return result;
    }

    public static string GetRelatedView(this Type viewModelType)
    {
        var result = viewModelType.Name;

        if (viewModelType.IsInterface && result.StartsWith("I"))
            result = result.Substring(1, result.Length - 1);

        result = result.ReplaceLastOf(GenericConstants.ViewModel, GenericConstants.View);

        return result;
    }

    public static string GetRelatedWindow(this Type viewModelType)
    {
        var result = viewModelType.Name;

        if (viewModelType.IsInterface && result.StartsWith("I"))
            result = result.Substring(1, result.Length - 1);

        result = result.ReplaceLastOf(GenericConstants.ViewModel, GenericConstants.Window);

        return result;
    }

    public static string GetRelatedView(this IViewModel viewModel) =>
        viewModel.GetType().GetRelatedView();

    public static string GetRelatedWindow(this IViewModel viewModel) =>
        viewModel.GetType().GetRelatedWindow();

    public static string GetRelatedViewModel(this Type type)
    {
        var result = type.Name;

        if (type.IsInterface && result.StartsWith("I"))
            result = result.Substring(1, result.Length - 1);

        if (type.GetInterfaces().Contains(typeof(IWindow)))
            result = result.ReplaceLastOf(GenericConstants.Window, GenericConstants.ViewModel);
        else if (type.GetInterfaces().Contains(typeof(IView)))
            result = result.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel);

        return result;
    }

    public static string GetRelatedViewModel(this IView view) =>
        view.GetType().GetRelatedViewModel();

    public static string GetRelatedViewModel(this IWindow window) =>
        window.GetType().GetRelatedViewModel();

    /// <summary>
    /// Registers the application's view type map for AOT-compatible view resolution.
    /// Call this at application startup before any navigation occurs.
    /// </summary>
    /// <param name="viewTypeMap">
    /// A dictionary mapping view type names (e.g. <c>"UserView"</c>) to their <see cref="Type"/> objects.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewTypeMap"/> is <c>null</c>.</exception>
    /// <remarks>
    /// Registering a view type map enables AOT-safe view resolution in <see cref="GetRelatedViewType"/>.
    /// Without a registered map the method falls back to a runtime assembly scan that is not AOT-compatible.
    /// </remarks>
    public static void RegisterViewTypes(IReadOnlyDictionary<string, Type> viewTypeMap)
    {
        ArgumentNullException.ThrowIfNull(viewTypeMap);
        ViewTypeRegistry.Register(viewTypeMap);
    }

    /// <summary>
    /// Resolves a view <see cref="Type"/> by name using the registered view type map.
    /// Falls back to a runtime assembly scan when no map has been registered (non-AOT environments only).
    /// </summary>
    /// <param name="name">The view type name to resolve (e.g. <c>"UserView"</c>).</param>
    /// <returns>
    /// The matching <see cref="Type"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// In AOT-compiled applications, call <see cref="RegisterViewTypes"/> at startup to populate the registry.
    /// The assembly-scan fallback will be trimmed away by the linker and will not work under AOT.
    /// </remarks>
    [RequiresUnreferencedCode("Fallback assembly scanning is not AOT-compatible. " +
        "Call ViewModelExtensions.RegisterViewTypes() at startup to enable AOT-safe view resolution.")]
    public static Type? GetRelatedViewType(this string name)
    {
        // Fast AOT-safe path: use the registered map.
        var registered = ViewTypeRegistry.TryGet(name);
        if (registered is not null)
            return registered;

        // Slow reflection path (non-AOT only).
        return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assemblyFilter)
                .Reverse()
                .Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t is not null)
            ??
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assemblyFilter)
                .Reverse()
                .SelectMany(assembly => assembly.GetExportedTypes())
                .FirstOrDefault(t => t.Name.Equals(name));
    }

    /// <summary>
    /// Finds and cancels all cancelable commands in the ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel.</param>
    /// <remarks>
    /// This method uses runtime reflection to enumerate public properties of the ViewModel instance.
    /// It is not AOT-compatible. In AOT-published applications, suppress IL2026 with a justified pragma
    /// or implement explicit command cancellation in your ViewModel.
    /// </remarks>
    [RequiresUnreferencedCode("ViewModel command discovery uses runtime reflection over property types.")]
    public static void CancelAllCommands(this IViewModel viewModel)
    {
        if (viewModel == null) return;

        // Get all properties that are ICancellationAwareCommand
        var commandProperties = viewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(ICancellationAwareCommand).IsAssignableFrom(p.PropertyType));

        foreach (var property in commandProperties)
        {
            if (property.GetValue(viewModel) is ICancellationAwareCommand command &&
                command.IsCancellationSupported)
            {
                command.Cancel();
            }
        }
    }

    /// <summary>
    /// Resets the state of all commands in the ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel.</param>
    /// <remarks>
    /// This method uses runtime reflection to enumerate public properties of the ViewModel instance.
    /// It is not AOT-compatible. In AOT-published applications, suppress IL2026 with a justified pragma
    /// or implement explicit command state reset in your ViewModel.
    /// </remarks>
    [RequiresUnreferencedCode("ViewModel command discovery uses runtime reflection over property types.")]
    public static void ResetAllCommandStates(this IViewModel viewModel)
    {
        if (viewModel == null) return;

        // Get all properties that are ICommand
        var commandProperties = viewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(ICommand).IsAssignableFrom(p.PropertyType));

        foreach (var property in commandProperties)
        {
            if (property.GetValue(viewModel) is IAsyncRelayCommand command)
            {
                command.NotifyCanExecuteChanged();
            }
        }
    }
}
