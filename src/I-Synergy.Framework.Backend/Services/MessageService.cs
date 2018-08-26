using ISynergy.Interfaces;
using ISynergy.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class MessageService : IEmailSender, ISmsSender
    {
        public MessageService(IOptions<AuthMessageSenderOptions> optionsAccessor, IOptions<MessageServiceOptions> sender)
        {
            Options = optionsAccessor.Value;
            Sender = sender.Value;
        }

        public AuthMessageSenderOptions Options { get; }
        public MessageServiceOptions Sender { get; }

        public async Task SendEmailFromAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            await ExecuteFromAsync(Options.SendGridKey, subject, message, email);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            await ExecuteAsync(Options.SendGridKey, subject, message, email);
        }

        public async Task ExecuteAsync(string apiKey, string subject, string message, string email)
        {
            Argument.IsNotNullOrEmpty(nameof(MessageServiceOptions.EmailAddress), Sender.EmailAddress);
            Argument.IsNotNullOrEmpty(nameof(MessageServiceOptions.EmailSender), Sender.EmailSender);

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Sender.EmailAddress, Sender.EmailSender),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);
        }

        public async Task ExecuteFromAsync(string apiKey, string subject, string message, string email)
        {
            Argument.IsNotNullOrEmpty(nameof(MessageServiceOptions.EmailAddress), Sender.EmailAddress);
            Argument.IsNotNullOrEmpty(nameof(MessageServiceOptions.EmailSender), Sender.EmailSender);

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(email),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(Sender.EmailAddress, Sender.EmailSender));
            msg.AddBcc(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}