using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Models;
using ISynergy.Framework.Mail.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ISynergy.Framework.Mail.Services;

/// <summary>
/// Email service to send email through SendGrid.
/// </summary>
internal class MailService : IMailService
{
    private readonly MailOptions _sendGridOptions;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor of Email Service.
    /// </summary>
    /// <param name="sendGridOptions">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public MailService(
        IOptions<MailOptions> sendGridOptions,
        ILogger<MailService> logger)
    {
        _sendGridOptions = sendGridOptions.Value;
        _logger = logger;

        Argument.IsNotNullOrEmpty(_sendGridOptions.EmailAddress);
        Argument.IsNotNullOrEmpty(_sendGridOptions.Sender);
        Argument.IsNotNullOrEmpty(_sendGridOptions.Key);
    }

    /// <summary>
    /// Sends e-mail from external e-mail address(From) to other external e-mail address(To).
    /// </summary>
    /// <param name="emailMessage">The email message.</param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public async Task<bool> SendEmailAsync(MailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            // Plug in your email service here to send an email.
            var client = new SendGridClient(_sendGridOptions.Key);
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
                    personalization.Bccs = [];

                personalization.Bccs.Add(new EmailAddress(emailMessage.EmailAddressFrom));
            }

            personalization.Subject = emailMessage.Subject;

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(emailMessage.EmailAddressFrom),
                PlainTextContent = emailMessage.Message,
                HtmlContent = emailMessage.Message,
                Personalizations = [personalization]
            };

            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
