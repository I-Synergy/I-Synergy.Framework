using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface INavigationService
{
    event EventHandler? BackStackChanged;

    bool CanGoBack { get; }
    Task GoBackAsync();
    Task CleanBackStackAsync(bool suppressEvent = false);

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel;

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TView"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView;

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel;

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TView"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView;

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel;
}
