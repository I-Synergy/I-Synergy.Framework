using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface INavigationService
    /// </summary>
    public interface INavigationServiceExtended : INavigationService
    {
        object Frame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
        void OpenBlade(IViewModelBladeView owner, IViewModel viewmodel);
        void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);

        /// <summary>
        /// Cleans the back stack asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CleanBackStackAsync();

        void Configure(string key, Type pageType);
    }
}
