using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Senders
{
    /// <summary>
    /// Interface IEmailSender
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task SendEmailAsync(string email, string subject, string message);

        /// <summary>
        /// Sends the email from asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task SendEmailFromAsync(string email, string subject, string message);
    }
}
