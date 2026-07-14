using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISynergy.Framework.UI.Controls;

public class LoadingView : Page
{
    private readonly IApplicationLifecycleService _applicationLifecycleService;
    private readonly ICommonServices _commonServices;
    private Button? _signInButton;
    private bool _loadingCompleted;

    public LoadingView(ICommonServices commonServices, SplashScreenOptions splashScreenOptions)
    {
        ArgumentNullException.ThrowIfNull(commonServices);
        ArgumentNullException.ThrowIfNull(splashScreenOptions);

        _commonServices = commonServices;
        _applicationLifecycleService = Application.Services.GetRequiredService<IApplicationLifecycleService>();

        InitializeContent(splashScreenOptions);
        SetupEventHandlers();

        if (!string.IsNullOrEmpty(splashScreenOptions.Resource) && splashScreenOptions.SplashScreenType == SplashScreenTypes.Image)
        {
            if (FindName("BackgroundImage") is Image backgroundImage)
                backgroundImage.Source = new BitmapImage(new Uri(splashScreenOptions.Resource));
        }
    }

    public bool IsSignInButtonEnabled
    {
        get => _signInButton?.IsEnabled ?? false;
        set
        {
            if (_signInButton is not null)
                _signInButton.IsEnabled = value;
        }
    }

    public object SignInButtonText
    {
        get => _signInButton?.Content ?? string.Empty;
        set
        {
            if (_signInButton is not null)
                _signInButton.Content = value;
        }
    }

    public event EventHandler? LoadingCompleted;

    public void CompleteLoading()
    {
        _loadingCompleted = true;
        _applicationLifecycleService.SignalApplicationLoaded();
        LoadingCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeContent(SplashScreenOptions splashScreenOptions)
    {
        var overlay = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x00, 0x00))
        };

        _signInButton = new Button
        {
            Content = _commonServices.LanguageService.GetString("Skip"),
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            IsEnabled = false
        };

        if (Application.Current.Resources.Contains("SystemAccentColor") && Application.Current.Resources["SystemAccentColor"] is Color primaryColor)
            _signInButton.Background = new SolidColorBrush(primaryColor);

        var mainGrid = new Grid();

        if (splashScreenOptions.SplashScreenType == SplashScreenTypes.Image)
        {
            var backgroundImage = new Image
            {
                Name = "BackgroundImage",
                Stretch = Stretch.UniformToFill,
                Opacity = 1
            };
            mainGrid.Children.Add(backgroundImage);
        }

        var loadingSpinner = new LoadingSpinner
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Background = null
        };

        loadingSpinner.SetBinding(LoadingSpinner.MessageProperty, new Binding
        {
            Source = _commonServices.BusyService,
            Path = new PropertyPath(nameof(IBusyService.BusyMessage))
        });

        mainGrid.Children.Add(overlay);
        mainGrid.Children.Add(loadingSpinner);
        mainGrid.Children.Add(_signInButton);

        Content = mainGrid;
    }

    private void SetupEventHandlers()
    {
        if (_signInButton is not null)
            _signInButton.Click += OnSignInClicked;

        Loaded += OnSignalUIReady;
        Unloaded += OnUnloaded;

        _applicationLifecycleService.ApplicationInitialized += OnApplicationInitialized;
    }

    private void OnApplicationInitialized(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() => IsSignInButtonEnabled = true);
    }

    private void OnSignalUIReady(object? sender, RoutedEventArgs e)
    {
        try
        {
            _applicationLifecycleService.SignalApplicationUIReady();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error signaling UI ready: {ex}");
        }
    }

    private void OnSignInClicked(object? sender, RoutedEventArgs e)
    {
        if (IsSignInButtonEnabled)
            CompleteLoading();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (_signInButton is not null)
            _signInButton.Click -= OnSignInClicked;

        Loaded -= OnSignalUIReady;
        Unloaded -= OnUnloaded;

        if (_applicationLifecycleService is not null)
            _applicationLifecycleService.ApplicationInitialized -= OnApplicationInitialized;
    }

    public Task WaitForLoadingCompleteAsync()
    {
        if (_loadingCompleted)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource<bool>();

        EventHandler? handler = null;
        handler = (s, e) =>
        {
            LoadingCompleted -= handler;
            tcs.TrySetResult(true);
        };

        LoadingCompleted += handler;

        return tcs.Task;
    }
}
