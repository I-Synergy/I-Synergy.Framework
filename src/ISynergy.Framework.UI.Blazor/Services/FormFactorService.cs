using ISynergy.Framework.UI.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;
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

