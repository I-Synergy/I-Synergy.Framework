using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.UI.Extensions;
public static class MauiAppExtensions
{
    public static MauiApp SetLocatorProvider(this MauiApp app)
    {
        ServiceLocator.SetLocatorProvider(app.Services);
        return app;
    }
}
