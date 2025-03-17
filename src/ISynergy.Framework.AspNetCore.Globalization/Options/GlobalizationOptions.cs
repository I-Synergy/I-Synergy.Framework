using ISynergy.Framework.AspNetCore.Globalization.Enumerations;

namespace ISynergy.Framework.AspNetCore.Globalization.Options;

public class GlobalizationOptions
{
    public string DefaultCulture { get; set; }
    public string[] SupportedCultures { get; set; }
    public RequestCultureProviderTypes ProviderType { get; set; }
}
