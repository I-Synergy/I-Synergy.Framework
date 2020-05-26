using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IBusyService
    {
        Task StartBusyAsync();
        Task StartBusyAsync(string message);
        Task EndBusyAsync();
        string BusyMessage { get; set; }
        bool IsBusy { get; set; }
    }
}
