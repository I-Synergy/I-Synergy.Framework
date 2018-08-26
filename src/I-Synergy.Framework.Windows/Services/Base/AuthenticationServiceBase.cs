using CommonServiceLocator;
using ISynergy.Common;
using ISynergy.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract class AuthenticationServiceBase
    {
        protected IContext Context;
        protected IBusyService Busy;

        public AuthenticationServiceBase(IContext context, IBusyService busy)
        {
            Context = context;
            Busy = busy;
        }

        public abstract Task LogoutAsync();

        public async Task LoginAsync()
        {
            if (Context.CurrentProfile?.Identity == null || Context.CurrentProfile?.Identity.IsAuthenticated == false)
            {
                await Busy.StartBusyAsync();

                if (Context.CurrentProfile?.Token != null)
                {
                    Busy.BusyMessage = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Authentication_Authenticating");

                    var securityToken = new JwtSecurityToken(Context.CurrentProfile.Token.id_token);

                    Context.CurrentProfile.UserInfo = ClaimConverters.ConvertClaimsToUserInfo(securityToken.Claims);
                    Context.CurrentProfile.TokenExpiration = securityToken.ValidTo.ToLocalTime();

                    string timezoneId = Context.CurrentProfile.UserInfo.TimeZoneId;

                    if(string.IsNullOrEmpty(timezoneId))
                        timezoneId = TimeZoneInfo.Local.Id;

                    Context.CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);


                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_RefreshToken = Context.CurrentProfile.Token.refresh_token;

                    ServiceLocator.Current.GetInstance<ITelemetryService>().Id = Context.CurrentProfile.UserInfo.Username;
                    ServiceLocator.Current.GetInstance<ITelemetryService>().Account_Id = Context.CurrentProfile.UserInfo.User_Id.ToString();

                    SetPrincipal(Context.CurrentProfile.UserInfo.Username, Context.CurrentProfile.UserInfo.Roles.ToArray());

                    if(Context.Profiles.Any(q => q.Username == Context.CurrentProfile.Username))
                        Context.Profiles.Remove(Context.Profiles.Single(q => q.Username == Context.CurrentProfile.Username));

                    Context.Profiles.Add(Context.CurrentProfile);

                    // Algemene data ophalen
                    Busy.BusyMessage = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Authentication_Retrieve_masterdata");

                    //loading Masteritems
                    await LoadMasterItemsAsync();
                    
                    //Loading useritems
                    await LoadUserItemsAsync();

                    //Loading settings
                    Busy.BusyMessage = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Authentication_Retrieve_settings");

                    await LoadSettingsAsync();
                    await RefreshSettingsAsync();
                    await AuthenticationChangedAsync();
                }
                else
                {
                    await LogoutAsync();
                }

                await Busy.EndBusyAsync();
            }
            else
            {
                await LogoutAsync();
            }
        }

        protected abstract Task LoadMasterItemsAsync();
        protected abstract Task LoadUserItemsAsync();

        public abstract Task<bool> AuthenticationChangedAsync();

        public Task LoadSettingsAsync() => ServiceLocator.Current.GetInstance<ISettingsServiceBase>().LoadSettings();

        public abstract Task RefreshSettingsAsync();

        private void SetPrincipal(string username, string[] roles)
        {
            Context.CurrentProfile.Identity = new GenericIdentity(username);
            Context.CurrentProfile.Principal = new GenericPrincipal(Context.CurrentProfile.Identity, roles);
        }
    }
}
