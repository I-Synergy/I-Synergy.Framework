using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Controls;

[Singleton(true)]
public partial class BusyIndicatorControl : Grid
{
    private readonly IBaseCommonServices _commonServices;

    public BusyIndicatorControl(IBaseCommonServices commonServices)
    {
        _commonServices = commonServices;

        var isBusyBinding = new Binding();
        isBusyBinding.Source = _commonServices.BusyService;
        isBusyBinding.Path = new PropertyPath(nameof(_commonServices.BusyService.IsBusy));
        isBusyBinding.Mode = BindingMode.OneWay;

        var busyMessageBinding = new Binding();
        busyMessageBinding.Source = _commonServices.BusyService;
        busyMessageBinding.Path = new PropertyPath(nameof(_commonServices.BusyService.BusyMessage));
        busyMessageBinding.Mode = BindingMode.OneWay;

        var progressRing = new ProgressRing()
        {
            Height = 48,
            Width = 48
        };

        var textBlock = new TextBlock()
        {
            FontSize = 16
        };

        var stackPanel = new StackPanel()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children = { progressRing, textBlock }
        };

        Children.Add(stackPanel);

        BindingOperations.SetBinding(this, Grid.VisibilityProperty, isBusyBinding);
        BindingOperations.SetBinding(progressRing, ProgressRing.IsActiveProperty, isBusyBinding);
        BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, busyMessageBinding);
        BindingOperations.SetBinding(textBlock, TextBlock.VisibilityProperty, isBusyBinding);

        Loaded += (s, e) =>
            MessageService.Default.Register<ApplicationInitializedMessage>(this, (m) =>
            {
                MessageService.Default.Send(new ApplicationLoadedMessage());
            });

        Unloaded += (s, e) =>
            MessageService.Default.Unregister<ApplicationInitializedMessage>(this);
    }
}
