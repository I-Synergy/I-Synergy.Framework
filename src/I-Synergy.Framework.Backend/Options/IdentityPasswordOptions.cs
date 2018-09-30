using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ISynergy.Options
{
    public class IdentityPasswordOptions : PasswordOptions
    {
        public Regex RequiredRegexMatch { get; set; }
    }
}
