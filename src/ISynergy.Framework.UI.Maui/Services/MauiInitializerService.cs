using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.UI.Services;

internal class MauiInitializerService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services) =>
        ServiceLocator.SetLocatorProvider(services);
}
