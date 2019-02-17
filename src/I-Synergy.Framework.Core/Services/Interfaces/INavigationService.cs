using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface INavigationService
    {
        object Frame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
        bool Navigate(string pageKey, object parameter = null, object infoOverride = null);
        IView GetNavigationBlade(string pageKey, object parameter = null);
        void Configure(string key, Type pageType);
        string GetNameOfRegisteredPage(Type page);
        Task CleanBackStackAsync();
    }
}
