using ISynergy.Options;
using ISynergy.Senders;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ISynergy.Services.Base
{
    public abstract class BaseMessageService : IEmailSender, ISmsSender
    {
        protected BaseMessageService(IOptions<AuthMessageSenderOptions> optionsAccessor, IOptions<MessageServiceOptions> sender)
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

        public abstract Task ExecuteAsync(string apiKey, string subject, string message, string email);
        public abstract Task ExecuteFromAsync(string apiKey, string subject, string message, string email);

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}