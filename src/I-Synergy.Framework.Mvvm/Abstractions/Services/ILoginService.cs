using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface ILoginService
    {
        Task ProcessLoginRequestAsync();
        Task LogoutAsync();
        Task LoginAsync(string username, string password);
        Task LoadSettingsAsync();
        Task RefreshSettingsAsync();
    }
}
