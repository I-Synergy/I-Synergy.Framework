using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IAuthenticationService
    {
        Task ProcessLoginRequestAsync();
        Task LogoutAsync();
        Task LoginAsync();
        Task<bool> AuthenticationChangedAsync();
        void CheckLicense();
        Task LoadSettingsAsync();
        Task RefreshSettingsAsync();
    }
}
