using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IPubNubClientService
    {
        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
