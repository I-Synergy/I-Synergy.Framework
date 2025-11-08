using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Options;

public class SplashScreenOptions
{
    public SplashScreenOptions(SplashScreenTypes splashScreenType, string? resource = null)
    {
        SplashScreenType = splashScreenType;
        Resource = resource;
    }

    public string? Resource { get; }
    public SplashScreenTypes SplashScreenType { get; }
}
