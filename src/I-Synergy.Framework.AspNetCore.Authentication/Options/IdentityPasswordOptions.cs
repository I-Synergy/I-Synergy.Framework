using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.AspNetCore.Authentication.Options
{
    public class IdentityPasswordOptions : PasswordOptions
    {
        public Regex RequiredRegexMatch { get; set; } = new Regex(string.Empty);
    }
}
