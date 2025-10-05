using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.UI.Services;

internal class ResolverService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services) =>
        ServiceLocator.SetLocatorProvider(services);
}
