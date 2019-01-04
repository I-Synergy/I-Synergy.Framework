using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface ILoginService
    {
        Task ProcessLoginRequestAsync();
        Task LogoutAsync();
        Task LoginAsync(string username, string password);
        Task AuthenticationChangedAsync();
        void CheckLicense();
        Task LoadSettingsAsync();
        Task RefreshSettingsAsync();
    }
}
