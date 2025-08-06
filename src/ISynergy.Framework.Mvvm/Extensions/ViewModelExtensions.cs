using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Reflection;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Extensions;

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

    public static string GetRelatedView(this IViewModel viewModel) =>
        viewModel.GetType().GetRelatedView();

    public static string GetRelatedViewModel(this Type type)
    {
        var result = type.Name;

        if (type.IsInterface && result.StartsWith("I"))
            result = result.Substring(1, result.Length - 1);

        result = result.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel);

        return result;
    }

    public static string GetRelatedViewModel(this IView view) =>
        view.GetType().GetRelatedViewModel();


    public static Type? GetRelatedViewType(this string name)
    {
        return
            AppDomain.CurrentDomain.GetAssemblies()
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
