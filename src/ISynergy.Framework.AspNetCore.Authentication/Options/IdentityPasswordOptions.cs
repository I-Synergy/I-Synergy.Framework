using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.AspNetCore.Authentication.Options;

/// <summary>
/// Class IdentityPasswordOptions.
/// Implements the <see cref="PasswordOptions" />
/// </summary>
/// <seealso cref="PasswordOptions" />
public class IdentityPasswordOptions : PasswordOptions
{
    /// <summary>
    /// Gets or sets the required regex match.
    /// </summary>
    /// <value>The required regex match.</value>
    public Regex RequiredRegexMatch { get; set; } = new Regex(string.Empty);
}
