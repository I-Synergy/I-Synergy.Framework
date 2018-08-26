using ISynergy.Models.General;
using System;
using System.Security.Principal;

namespace ISynergy
{
    public class Profile
    {
        public IIdentity Identity { get; set; }
        public IPrincipal Principal { get; set; }
        public UserInfo UserInfo { get; set; }
        public string Username { get; set; }
        public DateTime TokenExpiration { get; set; }
        public Token Token { get; set; }
    }
}
