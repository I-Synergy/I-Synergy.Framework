using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Options;

namespace ISynergy.Framework.UI.Controls;

/// https://github.com/CommunityToolkit/Maui/issues/2608

/// <summary>
/// A reusable loading view control that displays a busy state with optional media background support.
/// This control provides a standardized loading experience following the same pattern as EmptyView.
/// </summary>
[Lifetime(Lifetimes.Singleton)]
public class LoadingView : ContentPage
{
    private readonly IApplicationLifecycleService _applicationLifecycleService;
    private MediaElement? _backgroundMediaElement;
    private Image? _backgroundImage;
    private Button? _signInButton;
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

        _applicationLifecycleService = IPlatformApplication.Current!.Services.GetRequiredService<IApplicationLifecycleService>();

        InitializeContent(commonServices, splashScreenOptions);

        SetupEventHandlers();

        // Signal that the UI framework is ready for interaction.
        // At this point, the window is fully created and first page is loaded.
        // This is the earliest safe point for dialogs and modal navigation.
        Loaded += OnSignalUIReady;

        // Configure background based on splash screen type
        // Always set the default splash as the background image
        if (_backgroundImage is not null)
            _backgroundImage.Source = ImageSource.FromFile("splashscreen.png");

        // Override with custom image if explicitly specified
        if (!string.IsNullOrEmpty(splashScreenOptions.Resource) && splashScreenOptions.SplashScreenType == SplashScreenTypes.Image)
        {
            if (_backgroundImage is not null)
                _backgroundImage.Source = ImageSource.FromResource(splashScreenOptions.Resource);
        }
        // Override with custom video if explicitly specified
        else if (!string.IsNullOrEmpty(splashScreenOptions.Resource) && splashScreenOptions.SplashScreenType == SplashScreenTypes.Video)
        {
            if (_backgroundMediaElement is not null)
                _backgroundMediaElement.Source = MediaSource.FromResource(splashScreenOptions.Resource);
        }
    }

    /// <summary>
    /// Gets or sets the media source for the background media element.
    /// </summary>
    public MediaSource? MediaSource
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
    public string SignInButtonText
    {
        get => _signInButton?.Text ?? string.Empty;
        set
        {
            if (_signInButton is not null)
                _signInButton.Text = value;
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
        if (_backgroundMediaElement is not null && _backgroundMediaElement.CurrentState != MediaElementState.Stopped)
            _backgroundMediaElement.Pause();

        _loadingCompleted = true;
        _applicationLifecycleService.SignalApplicationLoaded();
        LoadingCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Initializes the view content and layout.
    /// </summary>
    private void InitializeContent(ICommonServices _, SplashScreenOptions splashScreenOptions)
    {
        // Always create background image for the default splash
        _backgroundImage = new Image
        {
            Aspect = splashScreenOptions.SplashScreenType == SplashScreenTypes.Image ? Aspect.Fill : Aspect.Center,
            IsOpaque = true
        };

        if (splashScreenOptions.SplashScreenType == SplashScreenTypes.None)
            _backgroundImage.VerticalOptions = LayoutOptions.Start;

        // Create media element only for Video splash screens
        if (splashScreenOptions.SplashScreenType == SplashScreenTypes.Video)
        {
            _backgroundMediaElement = new MediaElement
            {
                Aspect = Aspect.AspectFill,
                IsEnabled = false,
                ShouldShowPlaybackControls = false
            };
        }

        // Create semi-transparent overlay
        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#40000000")
        };

        // Create sign-in/skip button (only for Video splash — no skip needed for Image/None)
        _signInButton = new Button
        {
            Text = "Skip",
            Margin = new Thickness(20),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            IsEnabled = false,
            IsVisible = splashScreenOptions.SplashScreenType == SplashScreenTypes.Video
        };
        _signInButton.SetDynamicResource(Button.BackgroundColorProperty, "Primary");

        // Create main grid
        var mainGrid = new Grid();

        // Always add the background image (default splash or custom image)
        if (_backgroundImage is not null) // NOSONAR
        {
            mainGrid.Add(_backgroundImage);
        }

        // Add media element on top if present (video overlays the default splash background)
        if (_backgroundMediaElement is not null)
        {
            mainGrid.Add(_backgroundMediaElement);
        }

        mainGrid.Add(overlay);
        mainGrid.Add(new BusyIndicator());
        mainGrid.Add(_signInButton);

        this.Content = mainGrid;
    }

    /// <summary>
    /// Sets up event handlers for the view lifecycle and user interactions.
    /// </summary>
    private void SetupEventHandlers()
    {
        if (_signInButton is not null)
            _signInButton.Clicked += OnSignInClicked;

        Loaded += OnViewLoaded;
        Unloaded += OnViewUnloaded;

        // Subscribe to application lifecycle events
        _applicationLifecycleService.ApplicationInitialized += OnApplicationInitialized;
    }

    /// <summary>
    /// Handles the ApplicationInitialized event from the lifecycle service.
    /// </summary>
    private void OnApplicationInitialized(object? sender, EventArgs e)
    {
        if (MainThread.IsMainThread)
            IsSignInButtonEnabled = true;
        else
            MainThread.BeginInvokeOnMainThread(() => IsSignInButtonEnabled = true);
    }

    /// <summary>
    /// Handles the media ended event.
    /// </summary>
    private void OnMediaEnded(object? sender, EventArgs e)
    {
        CompleteLoading();
    }

    /// <summary>
    /// Handles the view loaded event.
    /// </summary>
    private void OnViewLoaded(object? sender, EventArgs e)
    {
        if (_backgroundMediaElement is not null)
        {
            _backgroundMediaElement.MediaEnded += OnMediaEnded;
            _backgroundMediaElement.Play();
        }
    }

    /// <summary>
    /// Handles the view loaded event to signal the UI is ready.
    /// </summary>
    private void OnSignalUIReady(object? sender, EventArgs e) => SignalUIReady();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SignalUIReady();
    }

    private void SignalUIReady()
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
    private void OnViewUnloaded(object? sender, EventArgs e)
    {
        if (_backgroundMediaElement is not null)
        {
            _backgroundMediaElement.MediaEnded -= OnMediaEnded;
            _backgroundMediaElement.Handler?.DisconnectHandler();
        }
    }

    /// <summary>
    /// Handles the sign-in button clicked event.
    /// </summary>
    private void OnSignInClicked(object? sender, EventArgs e)
    {
        if (IsSignInButtonEnabled)
            CompleteLoading();
    }

    /// <summary>
    /// Releases unmanaged and optionally managed resources.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe from events
        if (_signInButton is not null)
            _signInButton.Clicked -= OnSignInClicked;

        Loaded -= OnSignalUIReady;
        Loaded -= OnViewLoaded;
        Unloaded -= OnViewUnloaded;

        if (_backgroundMediaElement is not null)
            _backgroundMediaElement.MediaEnded -= OnMediaEnded;

        if (_applicationLifecycleService is not null)
            _applicationLifecycleService.ApplicationInitialized -= OnApplicationInitialized;
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
