using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IBusyService
    {
        Task StartBusyAsync(string message = null);
        Task EndBusyAsync();
        string BusyMessage { get; set; }
        bool IsBusy { get; set; }
    }
}
