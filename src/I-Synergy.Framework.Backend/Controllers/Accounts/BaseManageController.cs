using ISynergy.ViewModels.Manage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ISynergy.Senders;

namespace ISynergy.Controllers.Base
{
    [Authorize]
    public abstract class BaseManageController : Controller
    {
        protected IEmailSender EmailSender { get; }
        protected ISmsSender SmsSender { get; }
        protected ILogger Logger { get; }
        protected UrlEncoder UrlEncoder { get; }

        protected const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        protected BaseManageController(
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder)
        {
            EmailSender = emailSender;
            SmsSender = smsSender;
            Logger = loggerFactory.CreateLogger<BaseManageController>();
            UrlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public abstract Task<IActionResult> Profile();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Profile(ProfileViewModel model);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> SendVerificationEmail(ProfileViewModel model);

        [HttpGet]
        public abstract Task<IActionResult> ChangePassword();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> ChangePassword(ChangePasswordViewModel model);

        [HttpGet]
        public abstract Task<IActionResult> SetPassword();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> SetPassword(SetPasswordViewModel model);

        [HttpGet]
        public abstract Task<IActionResult> ExternalLogins();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> LinkLogin(string provider);

        [HttpGet]
        public abstract Task<IActionResult> LinkLoginCallback();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> RemoveLogin(RemoveLoginViewModel model);

        [HttpGet]
        public abstract Task<IActionResult> TwoFactorAuthentication();

        [HttpGet]
        public abstract Task<IActionResult> Disable2faWarning();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Disable2fa();

        [HttpGet]
        public abstract Task<IActionResult> EnableAuthenticator();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model);

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> ResetAuthenticator();

        [HttpGet]
        public abstract Task<IActionResult> GenerateRecoveryCodes();
        #region Helpers

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result?.Errors.EnsureNotNull())
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        protected static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        protected string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                UrlEncoder.Encode("WebApplication4"),
                UrlEncoder.Encode(email),
                unformattedKey);
        }
        #endregion Helpers
    }
}
