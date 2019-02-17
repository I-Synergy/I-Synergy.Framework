using ISynergy.Common;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract class BaseLoginService : ILoginService
    {
        public IContext Context { get; }
        public IBaseSettingsService SettingsService { get; }
        public IBusyService BusyService { get; }
        public ILanguageService LanguageService { get; }
        public ITelemetryService TelemetryService { get; }
        public INavigationService NavigationService { get; }
        public IAuthenticationService AuthenticationService { get; }

        public BaseLoginService(
            IContext context,
            IBaseSettingsService settingsService,
            IBusyService busyService,
            ILanguageService languageService,
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ITelemetryService telemetryService)
        {
            Context = context;
            SettingsService = settingsService;
            BusyService = busyService;
            LanguageService = languageService;
            NavigationService = navigationService;
            AuthenticationService = authenticationService;
            TelemetryService = telemetryService;
        }

        public async Task ProcessLoginRequestAsync()
        {
            if (Context.IsAuthenticated)
            {
                await LogoutAsync();
            }

            await NavigationService.CleanBackStackAsync();
        }

        public async Task LoginAsync(string username, string password)
        {
            // if username starts with "test:" or "local:"
            // remove this prefix and set environment to test.
            if(username.StartsWith(Constants.UsernamePrefixTest, StringComparison.InvariantCultureIgnoreCase))
            {
                username = username.Replace(Constants.UsernamePrefixTest, "", StringComparison.InvariantCultureIgnoreCase);
                Context.Environment = Enumerations.SoftwareEnvironments.Test;
            }
            // remove this prefix and set environment to local.
            else if (username.StartsWith(Constants.UsernamePrefixLocal, StringComparison.InvariantCultureIgnoreCase))
            {
                username = username.Replace(Constants.UsernamePrefixLocal, "", StringComparison.InvariantCultureIgnoreCase);
                Context.Environment = Enumerations.SoftwareEnvironments.Local;
            }
            else
            {
                Context.Environment = Enumerations.SoftwareEnvironments.Production;
            }
            
            if (SettingsService.User_AutoLogin && !string.IsNullOrEmpty(SettingsService.User_RefreshToken))
            {
                await AuthenticationService.AuthenticateWithRefreshTokenAsync(SettingsService.User_RefreshToken);

                if (Context.CurrentProfile?.Token != null)
                {
                    SettingsService.User_AutoLogin = true;
                }
                else
                {
                    SettingsService.User_AutoLogin = false;
                }
            }
            else
            {
                await AuthenticationService.AuthenticateWithTokenAsync(username, password);
            }

            if (Context.CurrentProfile?.Identity is null || Context.CurrentProfile?.Identity.IsAuthenticated == false)
            {
                await BusyService.StartBusyAsync();

                if (Context.CurrentProfile?.Token != null)
                {
                    BusyService.BusyMessage = LanguageService.GetString("Authentication_Authenticating");

                    var securityToken = new JwtSecurityToken(Context.CurrentProfile.Token.id_token);

                    Context.CurrentProfile.UserInfo = ClaimConverters.ConvertClaimsToUserInfo(securityToken.Claims);
                    Context.CurrentProfile.TokenExpiration = securityToken.ValidTo.ToLocalTime();

                    string timezoneId = Context.CurrentProfile.UserInfo.TimeZoneId;

                    if (string.IsNullOrEmpty(timezoneId))
                        timezoneId = TimeZoneInfo.Local.Id;

                    Context.CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);


                    SettingsService.User_RefreshToken = Context.CurrentProfile.Token.refresh_token;

                    TelemetryService.UserId = Context.CurrentProfile.UserInfo.Username;
                    TelemetryService.AccountId = Context.CurrentProfile.UserInfo.User_Id.ToString();

                    SetPrincipal(Context.CurrentProfile.UserInfo.Username, Context.CurrentProfile.UserInfo.Roles.ToArray());

                    if (Context.Profiles.Any(q => q.Username == Context.CurrentProfile.Username))
                        Context.Profiles.Remove(Context.Profiles.Single(q => q.Username == Context.CurrentProfile.Username));

                    Context.Profiles.Add(Context.CurrentProfile);

                    // Algemene data ophalen
                    BusyService.BusyMessage = LanguageService.GetString("Authentication_Retrieve_masterdata");

                    //loading Masteritems
                    await LoadMasterItemsAsync();

                    //Loading useritems
                    await LoadUserItemsAsync();

                    //Loading settings
                    BusyService.BusyMessage = LanguageService.GetString("Authentication_Retrieve_settings");

                    await LoadSettingsAsync();
                    await RefreshSettingsAsync();
                    await AuthenticationChangedAsync();
                }
                else
                {
                    await LogoutAsync();
                }

                await BusyService.EndBusyAsync();
            }
            else
            {
                await LogoutAsync();
            }
        }

        public Task LoadSettingsAsync() => SettingsService.LoadSettingsAsync();

        public abstract Task LoadMasterItemsAsync();
        public abstract Task LoadUserItemsAsync();
        public abstract Task RefreshSettingsAsync();
        public abstract Task AuthenticationChangedAsync();
        public abstract void CheckLicense();
        public abstract Task LogoutAsync();

        private void SetPrincipal(string username, string[] roles)
        {
            Context.CurrentProfile.Identity = new GenericIdentity(username);
            Context.CurrentProfile.Principal = new GenericPrincipal(Context.CurrentProfile.Identity, roles);
        }
    }
}
