using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Senders
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}