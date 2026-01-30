using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Services;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
public class EmptyView : ContentPage
{
    public EmptyView(ICommonServices commonServices)
    {
        this.Content = new BusyIndicator();

        // Signal that the UI framework is ready for interaction.
        // At this point, the window is fully created and first page is loaded.
        // This is the earliest safe point for dialogs and modal navigation.
        Loaded += (s, e) =>
        {
            try
            {
                var lifecycleService = commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();
                lifecycleService.SignalApplicationUIReady();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error signaling UI ready: {ex}");
            }
        };
    }
}
