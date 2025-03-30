using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Configuration;
public class SplashScreenOptions
{
    public Func<Task<Stream>>? AssetStreamProvider { get; set; }
    public string? ContentType { get; set; }
    public SplashScreenTypes SplashScreenType { get; set; }
}
