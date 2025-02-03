using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Controls;

public partial class BusyIndicatorControl : Grid
{
    private readonly ICommonServices _commonServices;
    private TaskCompletionSource<bool> _taskCompletion;
    private Func<Task> _initializationTask;

    public BusyIndicatorControl(ICommonServices commonServices)
    {
        _commonServices = commonServices;
        _taskCompletion = new TaskCompletionSource<bool>();

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

        Loaded += BusyIndicatorControl_Loaded;
        Unloaded += BusyIndicatorControl_Unloaded;
    }

    public void Initialize(Func<Task> task)
    {
        _initializationTask = task;
    }

    public Task WaitForCompletionAsync() => _taskCompletion.Task;

    private async void BusyIndicatorControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (_initializationTask is not null)
        {
            try
            {
                await _initializationTask();
                _taskCompletion.SetResult(true);
            }
            catch (Exception ex)
            {
                _taskCompletion.SetException(ex);
            }
        }
    }

    private void BusyIndicatorControl_Unloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= BusyIndicatorControl_Loaded;
        Unloaded -= BusyIndicatorControl_Unloaded;
    }
}
