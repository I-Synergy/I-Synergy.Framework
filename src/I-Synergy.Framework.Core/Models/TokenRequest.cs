using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Models
{
    public class TokenRequest
    {
        public string Username { get; }
        public IEnumerable<KeyValuePair<string, string>> Claims { get; }
        public IEnumerable<string> Roles { get; }
        public TimeSpan Expiration { get; }

        public TokenRequest(string username,
          IEnumerable<KeyValuePair<string, string>> claims,
          IEnumerable<string> roles,
          TimeSpan expiration)
        {
            Username = username;
            Claims = claims;
            Roles = roles;
            Expiration = expiration;
        }
    }
}
