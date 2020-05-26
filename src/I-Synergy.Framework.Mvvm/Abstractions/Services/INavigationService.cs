using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface INavigationService
    {
        object Frame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
        Task<IView> NavigateAsync<TViewModel>(object parameter = null, object infoOverride = null) where TViewModel : class, IViewModel;
        Task RemoveBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel);
        Task OpenBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel);
        void Configure(string key, Type pageType);
        string GetNameOfRegisteredPage(Type page);
        Task CleanBackStackAsync();
    }
}
