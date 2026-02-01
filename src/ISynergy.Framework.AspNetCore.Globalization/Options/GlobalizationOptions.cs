using ISynergy.Framework.AspNetCore.Globalization.Enumerations;

namespace ISynergy.Framework.AspNetCore.Globalization.Options;

public class GlobalizationOptions
{
    public string DefaultCulture { get; set; } = string.Empty;
    public string[] SupportedCultures { get; set; } = Array.Empty<string>();
    public RequestCultureProviderTypes ProviderType { get; set; }
}
