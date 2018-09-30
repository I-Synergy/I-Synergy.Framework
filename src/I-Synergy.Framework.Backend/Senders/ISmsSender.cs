using System.Threading.Tasks;

namespace ISynergy.Senders
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}