using System;
using System.Collections.Generic;
using System.Text;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace Sample.Shared.Models
{
    public class Profile : IProfile
    {
        public Guid AccountId => Guid.NewGuid();

        public string AccountDescription => "Sample";

        public string TimeZoneId => "W. Europe Standard Time";

        public Guid UserId => Guid.NewGuid();

        public string Username => "Anonimous";

        public string Email => "user@demo.com";

        public List<string> Roles => new List<string>();

        public List<string> Modules => new List<string>();

        public DateTimeOffset LicenseExpration => throw new NotImplementedException();

        public int LicenseUsers => 1;

        public Token Token => throw new NotImplementedException();

        public DateTime Expiration => throw new NotImplementedException();

        public bool IsAuthenticated => true;

        public bool IsInRole(string role) => true;
    }
}
