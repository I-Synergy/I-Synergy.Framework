using System.Threading.Tasks;

namespace ISynergy.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task SendEmailFromAsync(string email, string subject, string message);
    }
}