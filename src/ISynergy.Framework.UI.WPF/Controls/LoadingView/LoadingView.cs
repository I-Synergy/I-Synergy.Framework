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
        // Theme the splash surface instead of leaving the page transparent over the system window
        // brush (which made the splash look grey whatever the selected theme).
        if (Application.Current?.TryFindResource(Application.SplashBackgroundBrushKey) is Brush)
            SetResourceReference(BackgroundProperty, Application.SplashBackgroundBrushKey);
        else
            Background = SystemColors.WindowBrush;

        var overlay = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x00, 0x00))
        };

        _signInButton = new Button
        {
            Content = _commonServices.LanguageService.GetString("Skip"),
            Margin = new Thickness(20),
            MinWidth = 96,
            Padding = new Thickness(14, 6, 14, 6),
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            IsEnabled = false
        };

        // Use the themed accent when one is available and pick a caption colour that stays readable
        // on it (light accents need dark ink). Without this the button kept the default grey chrome
        // and an inherited light caption, which was unreadable on the splash.
        if (ResolveAccentColor() is { } accentColor)
        {
            _signInButton.Background = new SolidColorBrush(accentColor);
            _signInButton.Foreground = GetContrastingBrush(accentColor);
        }

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

            // The scrim exists to keep the spinner/button readable over a splash image. Without an
            // image it would only wash out the themed surface, so it is only added for image splashes.
            mainGrid.Children.Add(overlay);
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

        mainGrid.Children.Add(loadingSpinner);
        mainGrid.Children.Add(_signInButton);

        Content = mainGrid;
    }

    /// <summary>
    /// Resolves the theme accent from the application resources, when one is available.
    /// </summary>
    private static Color? ResolveAccentColor()
    {
        if (Application.Current is not { } app)
            return null;

        if (app.TryFindResource("SystemAccentBrush") is SolidColorBrush accentBrush)
            return accentBrush.Color;

        if (app.TryFindResource("SystemAccentColor") is Color accentColor)
            return accentColor;

        if (app.TryFindResource("PrimaryBrush") is SolidColorBrush primaryBrush)
            return primaryBrush.Color;

        return app.TryFindResource("Primary") is Color primary ? primary : null;
    }

    /// <summary>
    /// Returns black or white, whichever stays readable on <paramref name="background"/>.
    /// </summary>
    private static Brush GetContrastingBrush(Color background)
    {
        static double Linearize(byte channel)
        {
            var value = channel / 255d;
            return value <= 0.03928d ? value / 12.92d : Math.Pow((value + 0.055d) / 1.055d, 2.4d);
        }

        var luminance = (0.2126d * Linearize(background.R))
            + (0.7152d * Linearize(background.G))
            + (0.0722d * Linearize(background.B));

        return luminance > 0.5d ? Brushes.Black : Brushes.White;
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
