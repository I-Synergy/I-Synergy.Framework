using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ISynergy.Framework.UI.Controls;

public partial class BladeView : UserControl, IDisposable
{
    #region Properties
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource),
            typeof(ObservableCollection<IView>),
            typeof(BladeView),
            new PropertyMetadata(null, OnBladesChanged));

    public ObservableCollection<IView> ItemsSource
    {
        get { return (ObservableCollection<IView>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    private static void OnBladesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView)
        {
            // When old collection exists, unsubscribe
            if (e.OldValue is ObservableCollection<IView> oldCollection)
            {
                oldCollection.CollectionChanged -= bladeView.Blades_CollectionChanged;
            }

            // Subscribe to new collection
            if (e.NewValue is ObservableCollection<IView> newCollection)
            {
                newCollection.CollectionChanged += bladeView.Blades_CollectionChanged;
                bladeView.UpdateBladeVisibility();
            }
        }
    }

    public double DisabledOpacity
    {
        get { return (double)GetValue(DisabledOpacityProperty); }
        set { SetValue(DisabledOpacityProperty, value); }
    }

    public static readonly DependencyProperty DisabledOpacityProperty = DependencyProperty.Register(nameof(DisabledOpacity), typeof(double), typeof(BladeView), new PropertyMetadata(0.75));

    public Brush DisabledBackground
    {
        get { return (Brush)GetValue(DisabledBackgroundProperty); }
        set { SetValue(DisabledBackgroundProperty, value); }
    }

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(BladeView), new PropertyMetadata(null));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(BladeView), new PropertyMetadata(Orientation.Horizontal));

    public double BladeSpacing
    {
        get { return (double)GetValue(BladeSpacingProperty); }
        set { SetValue(BladeSpacingProperty, value); }
    }

    public static readonly DependencyProperty BladeSpacingProperty = DependencyProperty.Register(nameof(BladeSpacing), typeof(double), typeof(BladeView), new PropertyMetadata(10.0));

    public HorizontalAlignment HorizontalBladeAlignment
    {
        get { return (HorizontalAlignment)GetValue(HorizontalBladeAlignmentProperty); }
        set { SetValue(HorizontalBladeAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HorizontalBladeAlignmentProperty = DependencyProperty.Register(nameof(HorizontalBladeAlignment), typeof(HorizontalAlignment), typeof(BladeView), new PropertyMetadata(HorizontalAlignment.Right, OnHorizontalBladeAlignmentChanged));

    /// <summary>
    /// Gets or sets the logical edge the blade pane is anchored to. Logical means the edge follows
    /// the active flow direction: <see cref="BladePaneAnchor.Start"/> is the left edge in
    /// left-to-right cultures and the right edge in right-to-left cultures.
    /// </summary>
    public BladePaneAnchor Anchor
    {
        get { return (BladePaneAnchor)GetValue(AnchorProperty); }
        set { SetValue(AnchorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Anchor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnchorProperty = DependencyProperty.Register(nameof(Anchor), typeof(BladePaneAnchor), typeof(BladeView), new PropertyMetadata(BladePaneAnchor.Auto, OnAnchorChanged));

    /// <summary>
    /// Gets the resolved, physical horizontal alignment of the blade pane. Unlike
    /// <see cref="HorizontalBladeAlignment"/>, which describes the left-to-right intent, this value is
    /// already mirrored for right-to-left flow directions. It is consumed by the control template and
    /// must never be hardcoded in XAML.
    /// </summary>
    public HorizontalAlignment BladeAlignment
    {
        get { return (HorizontalAlignment)GetValue(BladeAlignmentProperty); }
        private set { SetValue(BladeAlignmentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BladeAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BladeAlignmentProperty =
        DependencyProperty.Register(nameof(BladeAlignment),
            typeof(HorizontalAlignment),
            typeof(BladeView),
            new PropertyMetadata(HorizontalAlignment.Right));

    public Thickness InnerPadding
    {
        get { return (Thickness)GetValue(InnerMarginProperty); }
        set { SetValue(InnerMarginProperty, value); }
    }

    public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register(nameof(InnerPadding), typeof(Thickness), typeof(BladeView), new PropertyMetadata(new Thickness(8)));

    public double BladeHeight
    {
        get { return (double)GetValue(BladeHeightProperty); }
        set { SetValue(BladeHeightProperty, value); }
    }

    public static readonly DependencyProperty BladeHeightProperty = DependencyProperty.Register(nameof(BladeHeight), typeof(double), typeof(BladeView), new PropertyMetadata(0d));

    /// <summary>
    /// Gets or sets the width of a blade card.
    /// </summary>
    /// <remarks>
    /// The host owns the width so every blade in the pane has the same footprint; a blade page that
    /// declares its own width would otherwise make the cards inconsistent. The value includes the
    /// card's inner padding, so the content area is this width minus the content presenter margins.
    /// </remarks>
    public double BladeWidth
    {
        get { return (double)GetValue(BladeWidthProperty); }
        set { SetValue(BladeWidthProperty, value); }
    }

    public static readonly DependencyProperty BladeWidthProperty = DependencyProperty.Register(nameof(BladeWidth), typeof(double), typeof(BladeView), new PropertyMetadata(420d));

    public int AutoCollapseThreshold
    {
        get { return (int)GetValue(AutoCollapseThresholdProperty); }
        set { SetValue(AutoCollapseThresholdProperty, value); }
    }

    public static readonly DependencyProperty AutoCollapseThresholdProperty = DependencyProperty.Register(nameof(AutoCollapseThreshold), typeof(int), typeof(BladeView), new PropertyMetadata(0));

    #endregion

    private readonly Dictionary<IView, BladeState> _bladeStates = new();
    private readonly ILogger _logger = Application.Services.GetRequiredService<ILogger<BladeView>>();
    private bool _isInternalUpdate;
    private bool _isDisposed;
    private long _flowDirectionCallbackToken;

    private class BladeState
    {
        public bool IsRemoving { get; set; }
        public Guid MessageToken { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public BladeView()
    {
        InitializeComponent();
        InitializeDefaultValues();
        InitializeBladeManagement();
        InitializePlacement();
    }

    private void InitializeDefaultValues()
    {
        this.Visibility = Visibility.Collapsed;
        this.CornerRadius = new CornerRadius(8);
        this.Padding = new Thickness(8);
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.Orientation = Orientation.Horizontal;
        this.BladeSpacing = 10;
        this.DisabledOpacity = 0.75;
    }

    private void BladeView_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Height > 0)
        {
            BladeHeight = e.NewSize.Height -
                (InnerPadding.Top + InnerPadding.Bottom + Margin.Top + Margin.Bottom);
        }

        ApplyPlacement();
    }

    #region Placement

    private static void OnAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.ApplyPlacement();
    }

    private static void OnHorizontalBladeAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.ApplyPlacement();
    }

    private void InitializePlacement()
    {
        // FlowDirection is inherited, so the ambient direction is only known once the control is
        // placed in a visual tree. Watch the property instead of reading it once in the constructor.
        _flowDirectionCallbackToken = RegisterPropertyChangedCallback(FlowDirectionProperty, OnFlowDirectionChanged);

        ApplyPlacement();
    }

    private void OnFlowDirectionChanged(DependencyObject sender, DependencyProperty dependencyProperty)
    {
        if (!_isDisposed)
            ApplyPlacement();
    }

    /// <summary>
    /// Resolves the physical edge of the blade pane from <see cref="Anchor"/> and the ambient
    /// inherited <see cref="Microsoft.UI.Xaml.FrameworkElement.FlowDirection"/>, and publishes it
    /// through <see cref="BladeAlignment"/>.
    /// </summary>
    private void ApplyPlacement()
    {
        if (_isDisposed)
            return;

        var isRtl = FlowDirection == FlowDirection.RightToLeft;

        if (Anchor == BladePaneAnchor.Auto)
            BladeAlignment = isRtl ? Mirror(HorizontalBladeAlignment) : HorizontalBladeAlignment;
        else
            BladeAlignment = BladePlacement.IsStartSide(Anchor, isRtl) ? HorizontalAlignment.Left : HorizontalAlignment.Right;
    }

    /// <summary>
    /// Mirrors a horizontal alignment for a right-to-left flow direction. Center and Stretch are
    /// direction agnostic and are returned unchanged.
    /// </summary>
    /// <param name="alignment">The alignment expressed for a left-to-right flow direction.</param>
    /// <returns>The mirrored alignment.</returns>
    private static HorizontalAlignment Mirror(HorizontalAlignment alignment) => alignment switch
    {
        HorizontalAlignment.Left => HorizontalAlignment.Right,
        HorizontalAlignment.Right => HorizontalAlignment.Left,
        _ => alignment
    };

    #endregion

    private void InitializeBladeManagement()
    {
        this.SizeChanged += BladeView_SizeChanged;
    }

    private void Blades_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            if (!_isInternalUpdate)
            {
                HandleCollectionChanged(e);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling blade collection change");
            ResetBladeStates();
        }
    }

    private void HandleCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (UIElement item in e.OldItems)
            {
                if (item is IView view)
                {
                    DetachBladeView(view);
                    UnregisterBlade(view);
                }
            }
        }

        if (e.NewItems is not null)
        {
            foreach (UIElement item in e.NewItems)
            {
                if (item is IView view)
                {
                    RegisterBlade(view);
                }
            }
        }

        UpdateBladeVisibility();
    }

    /// <summary>
    /// Detaches a blade page from the view model that is about to be disposed.
    /// </summary>
    /// <remarks>
    /// Blade views are registered as singletons (see <c>Controls.View</c>) while their view models are
    /// scoped and disposed together with the blade scope. Removing the blade from the owner's
    /// collection is the last moment where the page is still attached, so the data context is cleared
    /// here — otherwise a pending/queued binding update (e.g. a TwoWay ComboBox selection) can write
    /// into the already disposed view model and throw <see cref="ObjectDisposedException"/> on the
    /// next open of the same page.
    /// </remarks>
    private void DetachBladeView(IView view)
    {
        if (view is FrameworkElement element)
            element.DataContext = null;
    }

    private void RegisterBlade(IView view)
    {
        if (view.ViewModel == null) return;

        var token = Guid.NewGuid();
        _bladeStates[view] = new BladeState
        {
            MessageToken = token,
            RegisteredAt = DateTime.UtcNow
        };
    }

    private void UnregisterBlade(IView view)
    {
        if (_bladeStates.ContainsKey(view))
        {
            _bladeStates.Remove(view);
        }
    }

    private void RemoveBlade(IView view)
    {
        if (!_bladeStates.TryGetValue(view, out var state) || state.IsRemoving)
            return;

        try
        {
            state.IsRemoving = true;
            _isInternalUpdate = true;

            if (this.DataContext is IViewModelBladeView bladeView)
            {
                bladeView.Blades?.Remove(view);
            }

            // Blade views are registered as singletons (see Controls.View), while the blade's view
            // model is scoped and disposed together with the blade scope. Detach the page from it so
            // that a binding cannot write into the disposed instance if the same page is shown again
            // before the service attaches the next view model.
            DetachBladeView(view);

            // Do NOT dispose the blade view or its view model here.
            // - Blade views are registered as singletons, so disposing one makes the next open of
            //   the same blade use a disposed page.
            // - The view model belongs to the blade's DI scope; the scope disposes it after the
            //   blade has been removed. Disposing it here left the (still bound) page pointing at a
            //   disposed instance, and the next DataContext switch made TwoWay bindings write into
            //   that disposed view model (ObjectDisposedException in SetValue).

            UnregisterBlade(view);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing blade");
        }
        finally
        {
            _isInternalUpdate = false;
            UpdateBladeVisibility();
        }
    }

    private void UpdateBladeVisibility()
    {
        if (this.DataContext is not IViewModelBladeView bladeView)
            return;

        this.Visibility = bladeView.Blades?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        if (bladeView.Blades?.Count > 0)
        {
            for (int i = 0; i < bladeView.Blades.Count; i++)
            {
                var blade = bladeView.Blades[i];
                if (blade is UIElement element)
                {
                    element.Opacity = i == bladeView.Blades.Count - 1 ? 1.0 : DisabledOpacity;
                }
            }
        }
    }

    private void ResetBladeStates()
    {
        _isInternalUpdate = false;
        foreach (var state in _bladeStates.Values)
        {
            state.IsRemoving = false;
        }
        UpdateBladeVisibility();
    }

    private void CleanupBlades()
    {
        if (this.DataContext is IViewModelBladeView bladeView)
        {
            foreach (var view in bladeView.Blades?.OfType<IView>().ToList() ?? new List<IView>())
            {
                RemoveBlade(view);
            }

            if (bladeView.Blades is ObservableCollection<IView> collection)
            {
                collection.CollectionChanged -= Blades_CollectionChanged;
            }
        }

        _bladeStates.Clear();
    }

    protected void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        CleanupBlades();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            this.SizeChanged -= BladeView_SizeChanged;
            UnregisterPropertyChangedCallback(FlowDirectionProperty, _flowDirectionCallbackToken);
            CleanupBlades();
        }

        _isDisposed = true;
    }
}