using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface INavigationService
{
    event EventHandler? BackStackChanged;

    bool CanGoBack { get; }
    Task GoBackAsync();
    void CleanBackStack(bool suppressEvent = false);

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

    /// <summary>
    /// Opens blade with a view retrieved from DI.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel);

    /// <summary>
    /// Opens blade with a custom defined view.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <param name="owner"></param>
    /// <param name="viewmodel"></param>
    /// <returns></returns>
    Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView;

    /// <summary>
    /// Removes the blade asynchronous.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);
}
