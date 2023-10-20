using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Controls
{
    public partial class BusyIndicatorControl : Grid
    {
        public BusyIndicatorControl()
        {
            var isBusyBinding = new Binding();
            isBusyBinding.Source = ServiceLocator.Default.GetInstance<IBusyService>();
            isBusyBinding.Path = new PropertyPath(nameof(IBusyService.IsBusy));
            isBusyBinding.Mode = BindingMode.OneWay;

            var busyMessageBinding = new Binding();
            busyMessageBinding.Source = ServiceLocator.Default.GetInstance<IBusyService>();
            busyMessageBinding.Path = new PropertyPath(nameof(IBusyService.BusyMessage));
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
        }
    }
}
