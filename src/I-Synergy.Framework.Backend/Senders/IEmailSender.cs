using System.Threading.Tasks;

namespace ISynergy.Senders
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task SendEmailFromAsync(string email, string subject, string message);
    }
}