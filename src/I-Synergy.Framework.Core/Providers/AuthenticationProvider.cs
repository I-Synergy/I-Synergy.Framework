using System;
using System.Linq;
using System.Windows.Input;

namespace ISynergy.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        public IContext Context { get; private set; }

        public AuthenticationProvider(IContext context)
        {
            Context = context;
        }

        public bool CanCommandBeExecuted(ICommand RelayCommand, object commandParameter)
        {
            return true;
        }

        public bool HasAccessToUIElement(object element, object tag, string authorizationTag)
        {
            string authorizationTagString = authorizationTag as string;

            if (authorizationTag != null && Context.CurrentProfile?.UserInfo != null)
            {
                string[] roles = authorizationTag.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (Context.CurrentProfile.UserInfo.Roles.Intersect(roles).Any()) return true;
            }

            return false;
        }
    }
}