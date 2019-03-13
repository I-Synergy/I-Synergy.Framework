using ISynergy.Models.Accounts;
using ISynergy.Options;
using ISynergy.Senders;
using ISynergy.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Controllers.Base
{
    [Authorize]
    public abstract class BaseAccountController : Controller
    {
        protected IEmailSender EmailSender { get; }
        protected ISmsSender SmsSender { get; }
        protected WebsiteOptions Websites { get; }
        protected ILogger Logger { get; }

        protected BaseAccountController(
            IEmailSender emailSender,
            ISmsSender smsSender,
            IOptions<WebsiteOptions> websiteSettings,
            ILoggerFactory loggerFactory)
        {
            EmailSender = emailSender;
            SmsSender = smsSender;
            Websites = websiteSettings.Value;
            Logger = loggerFactory.CreateLogger<BaseAccountController>();
        }

        [TempData]
        public string ErrorMessage { get; set; }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Login(LoginViewModel model, string returnUrl = null);

        [HttpGet]
        [AllowAnonymous]
        public abstract Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null);

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null);

        [HttpGet]
        [AllowAnonymous]
        public abstract Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null);

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null);

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                if (await RegisterExternal(new RegistrationData
                {
                    ApplicationId = model.ApplicationId,
                    LicenseName = model.Licensename,
                    Email = model.Email,
                    Password = model.Password,
                    Modules = model.Modules,
                    UsersAllowed = model.Users
                })
                )
                {
                    // Comment out following line to prevent a new user automatically logged on.
                    // await Manager.SignInManager.SignInAsync(user, isPersistent: false);
                    return Redirect(Websites.HomePage);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> Logout();

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract IActionResult ExternalLogin(string provider, string returnUrl = null);

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public abstract Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null);

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null);

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public abstract Task<IActionResult> ConfirmEmail(string userId, string code);

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model);

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code is null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> ResetPassword(ResetPasswordViewModel model);

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result?.Errors.EnsureNotNull())
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        protected IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect(Websites.HomePage);
            }
        }
        #endregion Helpers

        [HttpGet("account/accounts")]
        [Authorize(Roles = "License_Manager, License_Administrator")]
        public abstract Task<List<AccountFull>> GetAccountsAsync(CancellationToken cancellationToken = default);

        [HttpPut("account/accounts")]
        [Authorize(Roles = "License_Manager, License_Administrator")]
        public abstract Task<int> UpdateAccountAsync([FromBody]AccountFull e);

        [HttpPut("account/accounts/{id}")]
        [Authorize(Roles = "License_Manager, License_Administrator")]
        public abstract Task<int> ToggleAccountActivationAsync(Guid id);

        [HttpDelete("account/accounts/{id}")]
        public abstract Task<int> RemoveAccountAsync(Guid id);

        [HttpGet("account/roles")]
        public abstract Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);

        [HttpPut("account/users/{id}")]
        public abstract Task<bool> ToggleUserLockAsync(string id);

        [HttpPut("account/users")]
        public abstract Task<int> UpdateUserAsync([FromBody]UserEdit e);

        [HttpDelete("account/users/{id}")]
        public abstract Task<int> RemoveUserAsync(string id);

        [HttpGet("account/users/{id}")]
        public abstract Task<List<User>> GetUsersFromAccountAsync(Guid id);

        [HttpPost("account/users")]
        public abstract Task<bool> AddUserAsync([FromBody]UserAdd user);

        [HttpGet("account/check/license/{name}")]
        [AllowAnonymous]
        public abstract Task<bool> CheckIfLicenseIsAvailableAsync(string name);

        [HttpGet("account/check/email/{email}")]
        [AllowAnonymous]
        public abstract Task<bool> CheckIfEmailIsAvailableAsync(string email);

        [HttpPost]
        [AllowAnonymous]
        public abstract Task<bool> RegisterExternal([FromBody]RegistrationData e);

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public abstract Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false);

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> SendCode(SendCodeViewModel model);

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public abstract Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null);

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public abstract Task<IActionResult> VerifyCode(VerifyCodeViewModel model);

        [HttpGet("account/forgotpassword/{email}")]
        [AllowAnonymous]
        public abstract Task<bool> ForgotPasswordExternal(string email);
    }
}
