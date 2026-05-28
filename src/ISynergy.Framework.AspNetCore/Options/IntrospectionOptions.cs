using ISynergy.Framework.Core.Options;

namespace ISynergy.Framework.AspNetCore.Options;
public class IntrospectionOptions : ClientApplicationOptions
{
    public string? Issuer { get; set; }
}
