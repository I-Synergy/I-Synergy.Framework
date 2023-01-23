using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface INavigationService
    /// </summary>
    public interface INavigationService
    {
        object Frame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
        void OpenBlade(IViewModelBladeView owner, IViewModel viewmodel);
        void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);

        /// <summary>
        /// Navigates the asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <returns>Task</returns>
        Task NavigateAsync<TViewModel>() 
            where TViewModel : class, IViewModel;
        
        Task NavigateAsync<TViewModel>(object parameter) 
            where TViewModel : class, IViewModel;

        Task NavigateAsync(Type viewModel);

        /// <summary>
        /// Cleans the back stack asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CleanBackStackAsync();

        void Configure(string key, Type pageType);
    }
}
