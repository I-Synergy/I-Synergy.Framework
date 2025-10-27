using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
internal class EmptyView : ContentPage
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

        // Notify that the initial UI is ready. Consumers can coordinate with
        // initialization to decide when to proceed.
        Loaded += (s, e) =>
            commonServices.MessengerService.Send(new ApplicationUiReadyMessage());
    }
}
