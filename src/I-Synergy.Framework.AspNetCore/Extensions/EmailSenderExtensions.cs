using ISynergy.Framework.AspNetCore.Senders;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}