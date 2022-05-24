using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface INavigationService
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        object Frame { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        bool CanGoBack { get; }
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        bool CanGoForward { get; }
        /// <summary>
        /// Goes the back.
        /// </summary>
        void GoBack();
        /// <summary>
        /// Goes the forward.
        /// </summary>
        void GoForward();
        /// <summary>
        /// Navigates the asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="infoOverride">The information override.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        Task<IView> NavigateAsync<TViewModel>(object parameter = null, object infoOverride = null) where TViewModel : class, IViewModel;
        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        Task RemoveBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel);
        /// <summary>
        /// Opens the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        Task OpenBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel);
        /// <summary>
        /// Configures the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pageType">Type of the page.</param>
        void Configure(string key, Type pageType);
        /// <summary>
        /// Gets the name of registered page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>System.String.</returns>
        string GetNameOfRegisteredPage(Type page);
        /// <summary>
        /// Cleans the back stack asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CleanBackStackAsync();
    }
}
