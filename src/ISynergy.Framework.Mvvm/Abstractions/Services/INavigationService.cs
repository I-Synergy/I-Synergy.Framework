using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface INavigationService
{
    /// <summary>
    /// Event handler when backstack is changed.
    /// </summary>
    event EventHandler BackStackChanged;

    /// <summary>
    /// Frame to navigate.
    /// </summary>
    object Frame { get; set; }

    bool CanGoBack { get; }
    bool CanGoForward { get; }

    Task GoBackAsync();
    Task GoForwardAsync();
    Task CleanBackStackAsync();

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="navigateBack"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel;

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="navigateBack"></param>
    /// <returns></returns>
    Task NavigateAsync<TViewModel>(object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel;

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task NavigateModalAsync<TViewModel>(object parameter = null) where TViewModel : class, IViewModel;

    /// <summary>
    /// open blade as an asynchronous operation.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel);

    /// <summary>
    /// Removes the blade asynchronous.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);
}
