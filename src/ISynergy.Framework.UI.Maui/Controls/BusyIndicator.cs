using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// A reusable indicator control that displays an activity indicator with an optional message and tinted overlay.
/// This control binds to the IBusyService to automatically show/hide based on busy state.
/// </summary>
public class BusyIndicator : ContentView
{
    private readonly Label _messageLabel;
    private readonly ActivityIndicator _activityIndicator;
    private readonly StackLayout _stackLayout;
    private readonly Grid _overlayBackground;
    private readonly Grid _rootGrid;

    /// <summary>
    /// Bindable property for ShowOverlay.
    /// </summary>
    public static readonly BindableProperty ShowOverlayProperty =
        BindableProperty.Create(
            nameof(ShowOverlay),
            typeof(bool),
            typeof(BusyIndicator),
            defaultValue: true,
            propertyChanged: OnShowOverlayChanged);

    /// <summary>
    /// Bindable property for OverlayOpacity.
    /// </summary>
    public static readonly BindableProperty OverlayOpacityProperty =
        BindableProperty.Create(
            nameof(OverlayOpacity),
            typeof(double),
            typeof(BusyIndicator),
            defaultValue: 0.5,
            propertyChanged: OnOverlayOpacityChanged);

    /// <summary>
    /// Bindable property for OverlayColor.
    /// </summary>
    public static readonly BindableProperty OverlayColorProperty =
        BindableProperty.Create(
            nameof(OverlayColor),
            typeof(Color),
            typeof(BusyIndicator),
            defaultValue: null,
            propertyChanged: OnOverlayColorChanged);

    /// <summary>
    /// Gets or sets whether the tinted overlay background is shown.
    /// </summary>
    public bool ShowOverlay
    {
        get => (bool)GetValue(ShowOverlayProperty);
        set => SetValue(ShowOverlayProperty, value);
    }

    /// <summary>
    /// Gets or sets the opacity of the overlay background (0.0 to 1.0).
    /// </summary>
    public double OverlayOpacity
    {
        get => (double)GetValue(OverlayOpacityProperty);
        set => SetValue(OverlayOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the overlay background.
    /// </summary>
    public Color OverlayColor
    {
        get => (Color)GetValue(OverlayColorProperty);
        set => SetValue(OverlayColorProperty, value);
    }

    private static void OnShowOverlayChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BusyIndicator indicator)
        {
            indicator._overlayBackground.IsVisible = (bool)newValue;
        }
    }

    private static void OnOverlayOpacityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BusyIndicator indicator)
        {
            indicator._overlayBackground.Opacity = (double)newValue;
        }
    }

    private static void OnOverlayColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BusyIndicator indicator && newValue is Color color)
        {
            indicator._overlayBackground.BackgroundColor = color;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyIndicator"/> class.
    /// </summary>
    public BusyIndicator()
    {
        _messageLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };
        _messageLabel.SetDynamicResource(Label.TextColorProperty, "Primary");

        _activityIndicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center
        };
        _activityIndicator.SetDynamicResource(ActivityIndicator.ColorProperty, "Primary");

        _stackLayout = new StackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 10,
            Children = { _messageLabel, _activityIndicator }
        };

        // Create the tinted overlay background
        _overlayBackground = new Grid
        {
            Opacity = OverlayOpacity,
            IsVisible = ShowOverlay
        };

        // Use dynamic resource "Tertiary" for overlay background color
        _overlayBackground.SetDynamicResource(Grid.BackgroundColorProperty, "TintedGray950");

        // Create root grid with overlay and indicator
        _rootGrid = new Grid();
        _rootGrid.Children.Add(_overlayBackground);
        _rootGrid.Children.Add(_stackLayout);

        Content = _rootGrid;

        BindToBusyService();
    }

    /// <summary>
    /// Binds the control to a busy service for automatic updates.
    /// </summary>
    public void BindToBusyService()
    {
        IBusyService busyService = ServiceLocator.Default.GetRequiredService<IBusyService>();

        _messageLabel.BindingContext = busyService;
        _messageLabel.SetBinding(Label.TextProperty, new Binding(nameof(IBusyService.BusyMessage), BindingMode.OneWay));
        _messageLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IBusyService.IsBusy), BindingMode.OneWay));

        _activityIndicator.BindingContext = busyService;
        _activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(nameof(IBusyService.IsBusy), BindingMode.OneWay));

        // Bind the root visibility to busy state
        this.BindingContext = busyService;
        this.SetBinding(IsVisibleProperty, new Binding(nameof(IBusyService.IsBusy), BindingMode.OneWay));
    }

    /// <summary>
    /// Gets or sets the text displayed in the message label.
    /// </summary>
    public string Message
    {
        get => _messageLabel.Text;
        set => _messageLabel.Text = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the activity indicator is running.
    /// </summary>
    public bool IsRunning
    {
        get => _activityIndicator.IsRunning;
        set => _activityIndicator.IsRunning = value;
    }

    /// <summary>
    /// Gets or sets the color of the activity indicator and message text.
    /// </summary>
    public Color IndicatorColor
    {
        get => _activityIndicator.Color;
        set
        {
            _activityIndicator.Color = value;
            _messageLabel.TextColor = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the message label is visible.
    /// </summary>
    public bool IsMessageVisible
    {
        get => _messageLabel.IsVisible;
        set => _messageLabel.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the orientation of the stack layout.
    /// </summary>
    public StackOrientation Orientation
    {
        get => _stackLayout.Orientation;
        set => _stackLayout.Orientation = value;
    }

    /// <summary>
    /// Gets or sets the spacing between the message and indicator.
    /// </summary>
    public double Spacing
    {
        get => _stackLayout.Spacing;
        set => _stackLayout.Spacing = value;
    }
}
