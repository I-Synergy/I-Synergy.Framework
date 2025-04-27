using ISynergy.Framework.Core.Options;

namespace ISynergy.Framework.AspNetCore.Options;
public class IntrospectionOptions : ApplicationOptions
{
    public string? Issuer { get; set; }
}
