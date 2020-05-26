using System;
using System.Linq;
using System.Windows.Input;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Windows.Abstractions.Providers;

namespace ISynergy.Framework.Windows.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        public IContext Context { get; }

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
            if (authorizationTag != null && Context.CurrentProfile != null)
            {
                var roles = authorizationTag.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (Context.CurrentProfile.Roles.Intersect(roles).Any()) return true;
            }

            return false;
        }
    }
}
