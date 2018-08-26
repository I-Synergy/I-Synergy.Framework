using System.Threading.Tasks;

namespace ISynergy.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}