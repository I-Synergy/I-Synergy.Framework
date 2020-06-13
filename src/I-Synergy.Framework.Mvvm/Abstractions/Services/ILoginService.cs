using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface ILoginService
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Processes the login request asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task ProcessLoginRequestAsync();
        /// <summary>
        /// Logouts the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task LogoutAsync();
        /// <summary>
        /// Loads the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task LoadSettingsAsync();
        /// <summary>
        /// Refreshes the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task RefreshSettingsAsync();
    }
}
