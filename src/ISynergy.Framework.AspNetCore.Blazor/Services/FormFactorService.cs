using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;

namespace ISynergy.Framework.AspNetCore.Blazor.Services;

public class FormFactorService : IFormFactorService
{
    public string GetFormFactor()
    {
        return "Web";
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}

