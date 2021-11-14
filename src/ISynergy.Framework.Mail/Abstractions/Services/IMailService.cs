namespace ISynergy.Framework.Mail.Abstractions.Services
{
    /// <summary>
    /// Email service to send email.
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Sends e-mail from external e-mail address(From) to other external e-mail address(To).
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        Task<bool> SendEmailAsync(MailMessage emailMessage);
    }
}
