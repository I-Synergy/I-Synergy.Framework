using Azure.Identity;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Models;
using ISynergy.Framework.Mail.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace ISynergy.Framework.Mail.Services;

/// <summary>
/// Class MailService.
/// Implements the <see cref="IMailService" />
/// </summary>
/// <seealso cref="IMailService" />
internal class MailService : IMailService
{
    /// <summary>
    /// The mail options
    /// </summary>
    private readonly MailOptions _mailOptions;
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor of Email Service.
    /// </summary>
    /// <param name="mailOptions">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public MailService(
        IOptions<MailOptions> mailOptions,
        ILogger logger)
    {
        _mailOptions = mailOptions.Value;
        _logger = logger;

        Argument.IsNotNullOrEmpty(_mailOptions.EmailAddress);
        Argument.IsNotNullOrEmpty(_mailOptions.Sender);
        Argument.IsNotNullOrEmpty(_mailOptions.ClientId);
        Argument.IsNotNullOrEmpty(_mailOptions.TenantId);
        Argument.IsNotNullOrEmpty(_mailOptions.ClientSecret);
    }

    /// <summary>
    /// Send email as an asynchronous operation.
    /// </summary>
    /// <param name="emailMessage">The email message.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async Task<bool> SendEmailAsync(MailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message();

            if (emailMessage.EmailAddressFrom is not null)
                message.From = new Recipient { EmailAddress = new EmailAddress { Address = emailMessage.EmailAddressFrom } };
            else
                message.From = new Recipient { EmailAddress = new EmailAddress { Address = _mailOptions.EmailAddress, Name = _mailOptions.Sender } };

            if (emailMessage.EmailAddressesTo.Count > 0)
            {
                var recipients = new List<Recipient>();

                foreach (var address in emailMessage.EmailAddressesTo.EnsureNotNull())
                    recipients.Add(new Recipient { EmailAddress = new EmailAddress { Address = address } });

                message.ToRecipients = recipients;
            }

            if (emailMessage.EmailAddressesCc.Count > 0)
            {
                var recipients = new List<Recipient>();

                foreach (var address in emailMessage.EmailAddressesCc.EnsureNotNull())
                    recipients.Add(new Recipient { EmailAddress = new EmailAddress { Address = address } });

                message.CcRecipients = recipients;
            }

            if (emailMessage.EmailAddressesBcc.Count > 0 || emailMessage.SendCopy)
            {
                var recipients = new List<Recipient>();

                foreach (var address in emailMessage.EmailAddressesBcc.EnsureNotNull())
                    recipients.Add(new Recipient { EmailAddress = new EmailAddress { Address = address } });

                if (emailMessage.SendCopy)
                    recipients.Add(message.From);

                message.BccRecipients = recipients;
            }

            message.Subject = emailMessage.Subject;

            message.Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = emailMessage.Message
            };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var credentials = new ClientSecretCredential(
                _mailOptions.TenantId,
                _mailOptions.ClientId,
                _mailOptions.ClientSecret,
                options);

            var client = new GraphServiceClient(credentials, _mailOptions.Scopes);
            var body = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await client.Users[_mailOptions.EmailAddress]
                .SendMail
                .PostAsync(body, cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
