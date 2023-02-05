using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services.Base
{
    public interface IBaseNavigationService
    {
        /// <summary>
        /// Navigates the asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <returns>Task</returns>
        Task NavigateAsync<TViewModel>()
            where TViewModel : class, IViewModel;

        Task NavigateAsync<TViewModel>(object parameter)
            where TViewModel : class, IViewModel;

        Task ReplaceMainWindowAsync<T>() where T : IView;
    }
}
