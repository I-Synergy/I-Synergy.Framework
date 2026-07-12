using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Constants;
using System.Globalization;

namespace Sample.Controls;

public partial class TitleBar
{
    private readonly IDispatcherTimer _clockTimer;

    public TitleBar()
    {
        InitializeComponent();

        var settingsService = IPlatformApplication.Current!.Services.GetRequiredService<ISettingsService>();
        var language = settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;
        var isRightToLeft = CultureInfo.GetCultureInfo(language).TextInfo.IsRightToLeft;

        if (isRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;

        Loaded += TitleBarLoaded;
        Unloaded += TitleBarUnloaded;

        _clockTimer = Dispatcher.CreateTimer();
        _clockTimer.Interval = TimeSpan.FromSeconds(1);

        SetTitle();
        SetClock();
    }

    private void TitleBarLoaded(object? sender, EventArgs e)
    {
        _clockTimer.Tick += ClockTimerCallback;
        _clockTimer.Start();
    }

    private void TitleBarUnloaded(object? sender, EventArgs e)
    {
        if (_clockTimer is not null)
        {
            _clockTimer.Stop();
            _clockTimer.Tick -= ClockTimerCallback;
        }
    }

    private void ClockTimerCallback(object? sender, object e) => SetClock();

    private void SetTitle()
    {
        var commonServices = IPlatformApplication.Current!.Services.GetRequiredService<ICommonServices>();

        if (commonServices is not null)
            Title = $"{commonServices.InfoService.ProductName} v{commonServices.InfoService.ProductVersion} ({Environment.GetEnvironmentVariable(nameof(Environment))})";

    }

    private void SetClock() =>
        Subtitle = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
}