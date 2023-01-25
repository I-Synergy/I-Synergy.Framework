using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Views;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels
{
    public class AuthenticationViewModel : ViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        public string Username
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        public string Password
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public Command Login_Command { get; set; }
        public Command Register_Command { get; set; }

        public AuthenticationViewModel(
            IContext context, 
            IBaseCommonServices commonServices,
            IAuthenticationService authenticationService,
            ILogger logger, 
            bool automaticValidation = false) 
            : base(context, commonServices, logger, automaticValidation)
        {
            _authenticationService = authenticationService;

            Login_Command = new Command(async () => await SignInAsync());
            Register_Command = new Command(Register);
        }

        private void Register() =>
            Application.Current.MainPage.ReplaceMainWindow<IRegistrationView>();

        private async Task SignInAsync()
        {
            Argument.IsNotNullOrEmpty(Username);
            Argument.IsNotNullOrEmpty(Password);

            await _authenticationService.AuthenticateWithUsernamePasswordAsync(Username, Password);
            
            if (Context.CurrentProfile is not null && Context.IsAuthenticated)
            {
                // Save Username in preferences if authentication is successfull.
                if (Preferences.ContainsKey(nameof(Profile.Username)))
                    Preferences.Remove(nameof(Profile.Username));

                Preferences.Set(nameof(Profile.Username), Username);
            }
        }
    }
}
