using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Models;
using ISynergy.Framework.Mail.SendGrid.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Mail.SendGrid.Services;

/// <summary>
/// Email service to send email through SendGrid.
/// </summary>
/// <remarks>
/// <para>
/// <strong>AOT/Trimming notice:</strong> SendGrid SDK v9.x uses <c>Newtonsoft.Json</c> for request and
/// response serialization, which is not AOT-compatible. Applications targeting
/// <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should use
/// <c>ISynergy.Framework.Mail.Microsoft365</c> instead.
/// </para>
/// </remarks>
[RequiresUnreferencedCode("SendGrid SDK uses Newtonsoft.Json for serialization, which is not AOT-compatible. Use a different mail provider for AOT publishing.")]
[RequiresDynamicCode("Newtonsoft.Json requires dynamic code generation.")]
internal class SendGridMailService : IMailService
{
    private readonly SendGridClient _sendGridClient;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor of Email Service.
    /// </summary>
    /// <param name="sendGridOptions">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public SendGridMailService(
        IOptions<SendGridMailOptions> sendGridOptions,
        ILogger<SendGridMailService> logger)
    {
        var options = sendGridOptions.Value;
        _logger = logger;

        Argument.IsNotNullOrEmpty(options.EmailAddress);
        Argument.IsNotNullOrEmpty(options.Sender);
        Argument.IsNotNullOrEmpty(options.Key);

        // Create the client once to avoid exposing the API key in stack traces on every call.
        _sendGridClient = new SendGridClient(options.Key);
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

            var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

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
