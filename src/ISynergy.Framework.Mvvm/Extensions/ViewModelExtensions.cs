using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Reflection;

namespace ISynergy.Framework.Mvvm.Extensions;

/// <summary>
/// ViewModel extensions.
/// </summary>
public static class ViewModelExtensions
{
    private static Func<Assembly, bool> assemblyFilter =>
        (x =>
        !x.FullName.StartsWith("System") &&
        !x.FullName.StartsWith("WinRT") &&
        !x.FullName.StartsWith("Microsoft") &&
        !x.FullName.StartsWith("Syncfusion") &&
        !x.FullName.StartsWith("Snippets") &&
        !x.FullName.StartsWith("Xamarin") &&
        !x.FullName.StartsWith("netstandard") &&
        !x.FullName.StartsWith("mscorlib"));

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
        var result = type.FullName;

        if (type.IsGenericType)
        {
            result = type.GetGenericTypeDefinition().FullName.Remove(type.GetGenericTypeDefinition().FullName.IndexOf('`'));
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


    public static Type GetRelatedViewType(this string name)
    {
        return
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assemblyFilter)
                .Reverse()
                    .Select(assembly => assembly.GetType(name))
                    .FirstOrDefault(t => t != null)
                ??
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assemblyFilter)
                    .Reverse()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(t => t.Name.Equals(name));
    }
}
