using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Abstractions.Services
{
    public interface INavigationService : IBaseNavigationService
    {
        object Frame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
        Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel);
        void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel);

        /// <summary>
        /// Cleans the back stack asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CleanBackStackAsync();
    }
}
