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
        var label = new Label();
        label.BindingContext = commonServices.BusyService;
        label.SetBinding(Label.TextProperty, new Binding(nameof(commonServices.BusyService.BusyMessage), BindingMode.OneWay));
        label.SetBinding(Label.IsVisibleProperty, new Binding(nameof(commonServices.BusyService.IsBusy), BindingMode.OneWay));
        label.SetDynamicResource(Label.TextColorProperty, "Primary");

        var indicator = new ActivityIndicator();
        indicator.BindingContext = commonServices.BusyService;
        indicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(nameof(commonServices.BusyService.IsBusy), BindingMode.OneWay));
        indicator.SetDynamicResource(ActivityIndicator.ColorProperty, "Primary");

        var stackLayout = new StackLayout();
        stackLayout.VerticalOptions = LayoutOptions.Center;
        stackLayout.HorizontalOptions = LayoutOptions.Center;
        stackLayout.Add(label);
        stackLayout.Add(indicator);

        this.Content = stackLayout;

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
