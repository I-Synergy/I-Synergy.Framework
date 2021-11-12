using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Models;
using ISynergy.Framework.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ISynergy.Services
{
    /// <summary>
    /// Email service to send email through SendGrid.
    /// </summary>
    public class MailService : IMailService
    {
        private readonly MailOptions _options;
        private readonly ILogger _logger;
        private readonly string _sendGridKey;

        /// <summary>
        /// Constructor of Email Service.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public MailService(
            IConfiguration configuration,
            IOptions<MailOptions> options,
            ILogger logger)
        {
            _options = options.Value;
            _logger = logger;

            Argument.IsNotNullOrEmpty(nameof(MailOptions.EmailAddress), _options.EmailAddress);
            Argument.IsNotNullOrEmpty(nameof(MailOptions.EmailSender), _options.EmailSender);

            _sendGridKey = configuration.GetValue<string>("SendGridKey");

            Argument.IsNotNullOrEmpty("SendGridKey", _sendGridKey);
        }

        /// <summary>
        /// Sends e-mail from external e-mail address(From) to other external e-mail address(To).
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> SendEmailAsync(MailMessage emailMessage)
        {
            bool result = false;

            try
            {
                // Plug in your email service here to send an email.
                var client = new SendGridClient(_sendGridKey);
                var personalization = new Personalization();

                if (emailMessage.EmailAddressesTo.Count > 0)
                    personalization.Tos = emailMessage.EmailAddressesTo.Select(s => new EmailAddress(s)).ToList();

                if (emailMessage.EmailAddressesCc.Count > 0)
                    personalization.Ccs = emailMessage.EmailAddressesCc.Select(s => new EmailAddress(s)).ToList();

                if (emailMessage.EmailAddressesBcc.Count > 0)
                    personalization.Bccs = emailMessage.EmailAddressesBcc.Select(s => new EmailAddress(s)).ToList();

                if (emailMessage.SendCopy)
                {
                    if (personalization.Bccs is null)
                        personalization.Bccs = new List<EmailAddress>();

                    personalization.Bccs.Add(new EmailAddress(emailMessage.EmailAddressFrom));
                }

                personalization.Subject = emailMessage.Subject;

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(emailMessage.EmailAddressFrom),
                    PlainTextContent = emailMessage.Message,
                    HtmlContent = emailMessage.Message,
                    Personalizations = new List<Personalization> { personalization }
                };

                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }
    }
}
