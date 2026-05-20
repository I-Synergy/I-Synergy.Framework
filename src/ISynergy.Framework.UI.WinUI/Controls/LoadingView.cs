using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Media.Playback;

namespace ISynergy.Framework.UI.Controls;

public class LoadingView : Page
{
    private readonly IApplicationLifecycleService _applicationLifecycleService;
    private readonly IBusyService _busyService;
    private MediaPlayerElement? _backgroundMediaElement;
    private Image? _backgroundImage;
    private Button? _signInButton;
    private TextBlock? _statusLabel;
    private bool _loadingCompleted;
    /// <summary>
    /// Initializes a new instance of the LoadingView class.
    /// </summary>
    /// <param name="commonServices">The common services instance providing access to framework services.</param>
    /// <param name="splashScreenOptions">Splash screen configuration including media source and type.</param>
    /// <exception cref="ArgumentNullException">Thrown when commonServices is null.</exception>
    public LoadingView(ICommonServices commonServices, SplashScreenOptions splashScreenOptions)
    {
        ArgumentNullException.ThrowIfNull(commonServices);
        ArgumentNullException.ThrowIfNull(splashScreenOptions);

        _applicationLifecycleService = commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();
        _busyService = commonServices.BusyService;

        InitializeContent(commonServices, splashScreenOptions);

        SetupEventHandlers();

        // Signal that the UI framework is ready for interaction.
        // At this point, the window is fully created and first page is loaded.
        // This is the earliest safe point for dialogs and modal navigation.
        Loaded += OnSignalUIReady;

        // Configure background based on splash screen type
        if (!string.IsNullOrEmpty(splashScreenOptions.Resource) && splashScreenOptions.SplashScreenType == SplashScreenTypes.Image)
        {
            // Use Image for image resources
            if (_backgroundImage is not null)
                _backgroundImage.Source = new BitmapImage
                {
                    UriSource = new Uri(splashScreenOptions.Resource)
                };
        }
        else if (!string.IsNullOrEmpty(splashScreenOptions.Resource) && splashScreenOptions.SplashScreenType == SplashScreenTypes.Video)
        {
            // Use MediaElement for video resources
            if (_backgroundMediaElement is not null)
                _backgroundMediaElement.Source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(splashScreenOptions.Resource));
        }
    }

    /// <summary>
    /// Gets or sets the media source for the background media element.
    /// </summary>
    public IMediaPlaybackSource? MediaSource
    {
        get => _backgroundMediaElement?.Source;
        set
        {
            if (_backgroundMediaElement is not null)
                _backgroundMediaElement.Source = value;
        }
    }

    /// <summary>
    /// Gets or sets the image source for the background image element.
    /// </summary>
    public ImageSource? ImageSource
    {
        get => _backgroundImage?.Source;
        set
        {
            if (_backgroundImage is not null)
                _backgroundImage.Source = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the sign-in/skip button is enabled.
    /// </summary>
    public bool IsSignInButtonEnabled
    {
        get => _signInButton?.IsEnabled ?? false;
        set
        {
            if (_signInButton is not null)
                _signInButton.IsEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the text displayed on the sign-in/skip button.
    /// </summary>
    public object SignInButtonText
    {
        get => _signInButton?.Content ?? string.Empty;
        set
        {
            if (_signInButton is not null)
                _signInButton.Content = value;
        }
    }

    /// <summary>
    /// Occurs when the loading is complete and the application should proceed.
    /// </summary>
    public event EventHandler? LoadingCompleted;

    /// <summary>
    /// Completes the loading process and signals the application lifecycle service.
    /// </summary>
    public void CompleteLoading()
    {
        if (_backgroundMediaElement?.MediaPlayer is { } player && player.CurrentState != MediaPlayerState.Stopped)
            player.Pause();

        _loadingCompleted = true;
        _applicationLifecycleService.SignalApplicationLoaded();
        LoadingCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Initializes the view content and layout.
    /// </summary>
    private void InitializeContent(ICommonServices _, SplashScreenOptions splashScreenOptions)
    {
        // Create background element based on splash screen type
        if (splashScreenOptions.SplashScreenType == SplashScreenTypes.Image)
        {
            _backgroundImage = new Image
            {
                Stretch = Stretch.UniformToFill,
                Opacity = 1
            };
        }
        else if (splashScreenOptions.SplashScreenType == SplashScreenTypes.Video)
        {
            _backgroundMediaElement = new MediaPlayerElement
            {
                Stretch = Stretch.UniformToFill,
                IsEnabled = false,
                AreTransportControlsEnabled = false
            };
        }

        // Create semi-transparent overlay
        var overlay = new Grid
        {
            Background = new SolidColorBrush(ColorHelper.HexStringToColor("#00000040"))
        };

        // Create sign-in/skip button
        _signInButton = new Button
        {
            Content = "Skip",
            Margin = new Microsoft.UI.Xaml.Thickness(20),
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Bottom,
            IsEnabled = false
        };

        if (Application.Current.Resources.TryGetValue("Primary", out var primaryResource) && primaryResource is Brush primaryBrush)
            _signInButton.Background = primaryBrush;

        // Create main grid
        var mainGrid = new Grid();

        // Add background element based on type
        if (_backgroundImage is not null) // NOSONAR
        {
            mainGrid.Children.Add(_backgroundImage);
        }
        else if (_backgroundMediaElement is not null)
        {
            mainGrid.Children.Add(_backgroundMediaElement);
        }

        _statusLabel = new TextBlock
        {
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
            TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
            FontSize = 14,
            Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
        };

        var loadingPanel = new StackPanel
        {
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
            Spacing = 0
        };
        loadingPanel.Children.Add(new ProgressRing
        {
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
        });
        loadingPanel.Children.Add(_statusLabel);

        mainGrid.Children.Add(overlay);
        mainGrid.Children.Add(loadingPanel);
        mainGrid.Children.Add(_signInButton);

        this.Content = mainGrid;
    }

    /// <summary>
    /// Sets up event handlers for the view lifecycle and user interactions.
    /// </summary>
    private void SetupEventHandlers()
    {
        if (_signInButton is not null)
            _signInButton.Click += OnSignInClicked;

        Loaded += OnViewLoaded;
        Unloaded += OnViewUnloaded;

        // Subscribe to application lifecycle events
        _applicationLifecycleService.ApplicationInitialized += OnApplicationInitialized;

        _busyService.PropertyChanged += OnBusyServicePropertyChanged;
    }

    /// <summary>
    /// Handles the ApplicationInitialized event from the lifecycle service.
    /// </summary>
    private void OnApplicationInitialized(object? sender, EventArgs e)
    {
        IsSignInButtonEnabled = true;
    }

    private void OnBusyServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IBusyService.BusyMessage))
            return;

        var message = _busyService.BusyMessage;
        DispatcherQueue.TryEnqueue(() =>
        {
            if (_statusLabel is not null)
                _statusLabel.Text = message;
        });
    }

    /// <summary>
    /// Handles the media ended event.
    /// </summary>
    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        CompleteLoading();
    }

    /// <summary>
    /// Handles the view loaded event.
    /// </summary>
    private void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (_backgroundMediaElement?.MediaPlayer is { } player)
        {
            player.MediaEnded += OnMediaEnded;
            player.Play();
        }
    }

    /// <summary>
    /// Handles the view loaded event to signal the UI is ready.
    /// </summary>
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

    /// <summary>
    /// Handles the view unloaded event.
    /// </summary>
    private void OnViewUnloaded(object? sender, RoutedEventArgs e)
    {
        if (_backgroundMediaElement?.MediaPlayer is { } player)
        {
            player.MediaEnded -= OnMediaEnded;
            player.Dispose();
        }
    }

    /// <summary>
    /// Handles the sign-in button clicked event.
    /// </summary>
    private void OnSignInClicked(object? sender, RoutedEventArgs e)
    {
        if (IsSignInButtonEnabled)
            CompleteLoading();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        // Unsubscribe from events
        if (_signInButton is not null)
            _signInButton.Click -= OnSignInClicked;

        Loaded -= OnSignalUIReady;
        Loaded -= OnViewLoaded;
        Unloaded -= OnViewUnloaded;

        if (_backgroundMediaElement?.MediaPlayer is { } player)
            player.MediaEnded -= OnMediaEnded;

        if (_applicationLifecycleService is not null)
            _applicationLifecycleService.ApplicationInitialized -= OnApplicationInitialized;

        _busyService.PropertyChanged -= OnBusyServicePropertyChanged;
    }

    /// <summary>
    /// Wait for the loading view to complete (video ends or user skips).
    /// </summary>
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
