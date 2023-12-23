using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Media;
using Button = Microsoft.UI.Xaml.Controls.Button;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// The Blade is used as a child in the BladeView
/// </summary>
[TemplatePart(Name = "CloseButton", Type = typeof(Button))]
[TemplatePart(Name = "EnlargeButton", Type = typeof(Button))]
public partial class BladeItem : Expander
{
    private Button _closeButton;
    private Button _enlargeButton;
    private double _normalModeWidth;
    private bool _loaded = false;

    /// <summary>
    /// Fires when the blade is opened or closed
    /// </summary>
    public event EventHandler<Visibility> VisibilityChanged;

    /// <summary>
    /// Identifies the <see cref="TitleBarVisibility"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleBarVisibilityProperty = DependencyProperty.Register(nameof(TitleBarVisibility), typeof(Visibility), typeof(BladeItem), new PropertyMetadata(default(Visibility)));

    /// <summary>
    /// Identifies the <see cref="TitleBarBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register(nameof(TitleBarBackground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Identifies the <see cref="CloseButtonBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(nameof(CloseButtonBackground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Identifies the <see cref="IsOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(BladeItem), new PropertyMetadata(true, IsOpenChangedCallback));

    /// <summary>
    /// Identifies the <see cref="CloseButtonForeground"/> dependency property
    /// </summary>
    public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(nameof(CloseButtonForeground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    private WeakReference<BladeView> _parentBladeView;

    /// <summary>
    /// Gets or sets the foreground color of the close button
    /// </summary>
    public Brush CloseButtonForeground
    {
        get { return (Brush)GetValue(CloseButtonForegroundProperty); }
        set { SetValue(CloseButtonForegroundProperty, value); }
    }

    /// <summary>
    /// Gets or sets the visibility of the title bar for this blade
    /// </summary>
    public Visibility TitleBarVisibility
    {
        get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
        set { SetValue(TitleBarVisibilityProperty, value); }
    }

    /// <summary>
    /// Gets or sets the background color of the title bar
    /// </summary>
    public Brush TitleBarBackground
    {
        get { return (Brush)GetValue(TitleBarBackgroundProperty); }
        set { SetValue(TitleBarBackgroundProperty, value); }
    }

    /// <summary>
    /// Gets or sets the background color of the default close button in the title bar
    /// </summary>
    public Brush CloseButtonBackground
    {
        get { return (Brush)GetValue(CloseButtonBackgroundProperty); }
        set { SetValue(CloseButtonBackgroundProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this blade is opened
    /// </summary>
    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }

    internal BladeView ParentBladeView
    {
        get
        {
            this._parentBladeView.TryGetTarget(out var bladeView);
            return bladeView;
        }
        set => this._parentBladeView = new WeakReference<BladeView>(value);
    }

    private static void IsOpenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        BladeItem bladeItem = (BladeItem)dependencyObject;
        bladeItem.Visibility = bladeItem.IsOpen ? Visibility.Visible : Visibility.Collapsed;
        bladeItem.VisibilityChanged?.Invoke(bladeItem, bladeItem.Visibility);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeItem"/> class.
    /// </summary>
    public BladeItem()
    {
        DefaultStyleKey = typeof(BladeItem);
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// Override default OnApplyTemplate to capture child controls
    /// </summary>
    protected override void OnApplyTemplate()
    {
        _loaded = true;
        base.OnApplyTemplate();

        _closeButton = GetTemplateChild("CloseButton") as Button;
        _enlargeButton = GetTemplateChild("EnlargeButton") as Button;

        if (_closeButton == null)
        {
            return;
        }

        _closeButton.Click -= CloseButton_Click;
        _closeButton.Click += CloseButton_Click;

        if (_enlargeButton == null)
        {
            return;
        }

        _enlargeButton.Click -= EnlargeButton_Click;
        _enlargeButton.Click += EnlargeButton_Click;
    }

    /// <inheritdoc/>
    protected override void OnExpanded(EventArgs args)
    {
        base.OnExpanded(args);
        if (_loaded)
        {
            Width = _normalModeWidth;
            VisualStateManager.GoToState(this, "Expanded", true);
            var name = "Collapse Blade";
            if (_enlargeButton != null)
            {
                AutomationProperties.SetName(_enlargeButton, name);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnCollapsed(EventArgs args)
    {
        base.OnCollapsed(args);
        if (_loaded)
        {
            Width = double.NaN;
            VisualStateManager.GoToState(this, "Collapsed", true);
            var name = "Expand Blade";
            if (_enlargeButton != null)
            {
                AutomationProperties.SetName(_enlargeButton, name);
            }
        }
    }

    /// <summary>
    /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
    /// </summary>
    /// <returns>An automation peer for this <see cref="BladeItem"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new BladeItemAutomationPeer(this);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
        if (IsExpanded)
        {
            _normalModeWidth = Width;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        IsOpen = false;
    }

    private void EnlargeButton_Click(object sender, RoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }
}
