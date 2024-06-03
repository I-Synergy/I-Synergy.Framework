using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;

namespace ISynergy.Framework.UI.Controls;

internal class EmptyView : ContentPage
{
    private readonly IBaseCommonServices _commonServices;

    public EmptyView(IBaseCommonServices commonServices)
    {
        _commonServices = commonServices;

        var label = new Label();
        label.BindingContext = _commonServices.BusyService;
        label.SetBinding(Label.TextProperty, new Binding(nameof(_commonServices.BusyService.BusyMessage), BindingMode.OneWay));
        label.SetBinding(Label.IsVisibleProperty, new Binding(nameof(_commonServices.BusyService.IsBusy), BindingMode.OneWay));
        label.SetDynamicResource(Label.TextColorProperty, "Primary"); 

        var indicator = new ActivityIndicator();
        indicator.BindingContext = _commonServices.BusyService;
        indicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(nameof(_commonServices.BusyService.IsBusy), BindingMode.OneWay));
        indicator.SetDynamicResource(ActivityIndicator.ColorProperty, "Primary");

        var stackLayout = new StackLayout();
        stackLayout.VerticalOptions = LayoutOptions.Center;
        stackLayout.HorizontalOptions = LayoutOptions.Center;
        stackLayout.Add(label);
        stackLayout.Add(indicator);

        this.Content = stackLayout;

        Loaded += (s, e) =>
            MessageService.Default.Register<ApplicationInitializedMessage>(this, (m) =>
            {
                MessageService.Default.Send(new ApplicationLoadedMessage());
            });

        Unloaded += (s,e) =>
            MessageService.Default.Unregister<ApplicationInitializedMessage>(this);
    }
}
