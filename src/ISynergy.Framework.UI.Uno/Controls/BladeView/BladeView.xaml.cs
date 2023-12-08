using ISynergy.Framework.UI.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

namespace ISynergy.Framework.UI.Controls;

public partial class BladeView : ItemsControl
{
    private ScrollViewer _scrollViewer;
    private readonly Dictionary<BladeItem, Size> _cachedBladeItemSizes = new Dictionary<BladeItem, Size>();

    /// <summary>
    /// Fires whenever a <see cref="BladeItem" /> is opened
    /// </summary>
    public event EventHandler<BladeItem> BladeOpened;

    /// <summary>
    /// Fires whenever a <see cref="BladeItem" /> is closed
    /// </summary>
    public event EventHandler<BladeItem> BladeClosed;

    /// <summary>
    /// Identifies the <see cref="ActiveBlades" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ActiveBladesProperty = DependencyProperty.Register(nameof(ActiveBlades), typeof(IList<BladeItem>), typeof(BladeView), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="BladeMode" /> attached property.
    /// </summary>
    public static readonly DependencyProperty BladeModeProperty = DependencyProperty.RegisterAttached(nameof(BladeMode), typeof(BladeMode), typeof(BladeView), new PropertyMetadata(BladeMode.Normal, OnBladeModeChanged));

    /// <summary>
    /// Identifies the <see cref="AutoCollapseCountThreshold" /> attached property.
    /// </summary>
    public static readonly DependencyProperty AutoCollapseCountThresholdProperty = DependencyProperty.RegisterAttached(nameof(AutoCollapseCountThreshold), typeof(int), typeof(BladeView), new PropertyMetadata(int.MaxValue, OnOpenBladesChanged));

    /// <summary>
    /// Gets or sets a collection of visible blades
    /// </summary>
    /// <value>The active blades.</value>
    public IList<BladeItem> ActiveBlades
    {
        get { return (IList<BladeItem>)GetValue(ActiveBladesProperty); }
        set { SetValue(ActiveBladesProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether blade mode (ex: whether blades are full screen or not)
    /// </summary>
    /// <value>The blade mode.</value>
    public BladeMode BladeMode
    {
        get { return (BladeMode)GetValue(BladeModeProperty); }
        set { SetValue(BladeModeProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating what the overflow amount should be to start auto collapsing blade items
    /// </summary>
    /// <value>The automatic collapse count threshold.</value>
    /// <example>
    /// For example we put AutoCollapseCountThreshold = 2
    /// This means that each time a blade is added to the bladeview collection,
    /// we will validate the amount of added blades that have a title bar visible.
    /// If this number get's bigger than AutoCollapseCountThreshold, we will collapse all blades but the last one
    /// </example>
    /// <remarks>We don't touch blade items that have no title bar</remarks>
    public int AutoCollapseCountThreshold
    {
        get { return (int)GetValue(AutoCollapseCountThresholdProperty); }
        set { SetValue(AutoCollapseCountThresholdProperty, value); }
    }

    /// <summary>
    /// Handles the <see cref="E:BladeModeChanged" /> event.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnBladeModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var bladeView = (BladeView)dependencyObject;
        var bladeScrollViewer = bladeView.GetScrollViewer();

        if (bladeView.BladeMode == BladeMode.Fullscreen)
        {
            // Cache previous values of blade items properties (width & height)
            bladeView._cachedBladeItemSizes.Clear();

            if (bladeView.Items != null)
            {
                foreach (var item in bladeView.Items)
                {
                    var bladeItem = bladeView.GetBladeItem(item);
                    bladeView._cachedBladeItemSizes.Add(bladeItem, new Size(bladeItem.Width, bladeItem.Height));
                }
            }

            VisualStateManager.GoToState(bladeView, "FullScreen", false);
        }

        if (bladeView.BladeMode == BladeMode.Normal)
        {
            // Reset blade items properties & clear cache
            foreach (var kvBladeItemSize in bladeView._cachedBladeItemSizes)
            {
                kvBladeItemSize.Key.Width = kvBladeItemSize.Value.Width;
                kvBladeItemSize.Key.Height = kvBladeItemSize.Value.Height;
            }

            bladeView._cachedBladeItemSizes.Clear();

            VisualStateManager.GoToState(bladeView, "Normal", false);
        }

        // Execute change of blade item size
        bladeView.AdjustBladeItemSize();
    }

    /// <summary>
    /// Handles the <see cref="E:OpenBladesChanged" /> event.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnOpenBladesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var bladeView = (BladeView)dependencyObject;
        bladeView.CycleBlades();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeView"/> class.
    /// </summary>
    public BladeView()
    {
        this.InitializeComponent();

        Items.VectorChanged += ItemsVectorChanged;
        Loaded += (sender, e) => AdjustBladeItemSize();
        SizeChanged += (sender, e) => AdjustBladeItemSize();
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        CycleBlades();
        AdjustBladeItemSize();
    }

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new BladeItem();
    }

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is BladeItem;
    }

    /// <inheritdoc/>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        var blade = element as BladeItem;
        if (blade != null)
        {
            blade.VisibilityChanged += BladeOnVisibilityChanged;
            blade.ParentBladeView = this;
        }

        base.PrepareContainerForItemOverride(element, item);
        CycleBlades();
    }

    /// <inheritdoc/>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        var blade = element as BladeItem;
        if (blade != null)
        {
            blade.VisibilityChanged -= BladeOnVisibilityChanged;
        }

        base.ClearContainerForItemOverride(element, item);
    }

    /// <summary>
    /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
    /// </summary>
    /// <returns>An automation peer for this <see cref="BladeView"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new BladeViewAutomationPeer(this);
    }

    private void CycleBlades()
    {
        ActiveBlades = new ObservableCollection<BladeItem>();
        foreach (var item in Items)
        {
            BladeItem blade = GetBladeItem(item);
            if (blade != null)
            {
                if (blade.IsOpen)
                {
                    ActiveBlades.Add(blade);
                }
            }
        }

        // For now we skip this feature when blade mode is set to fullscreen
        if (AutoCollapseCountThreshold > 0 && BladeMode != BladeMode.Fullscreen && ActiveBlades.Any())
        {
            var openBlades = ActiveBlades.Where(item => item.TitleBarVisibility == Visibility.Visible).ToList();
            if (openBlades.Count > AutoCollapseCountThreshold)
            {
                for (int i = 0; i < openBlades.Count - 1; i++)
                {
                    openBlades[i].IsExpanded = false;
                }
            }
        }
    }

    private BladeItem GetBladeItem(object item)
    {
        BladeItem blade = item as BladeItem;
        if (blade == null)
        {
            blade = (BladeItem)ContainerFromItem(item);
        }

        return blade;
    }

    private void BladeOnVisibilityChanged(object sender, Visibility visibility)
    {
        var blade = sender as BladeItem;

        if (visibility == Visibility.Visible)
        {
            if (Items == null)
            {
                return;
            }

            var item = ItemFromContainer(blade);
            Items.Remove(item);
            Items.Add(item);
            BladeOpened?.Invoke(this, blade);
            ActiveBlades.Add(blade);
            UpdateLayout();

            // Need to do this because of touch. See more information here: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/760#issuecomment-276466464
            Windows.System.DispatcherQueue.GetForCurrentThread().TryEnqueue(
                DispatcherQueuePriority.Low,
                () => GetScrollViewer()?.ChangeView(_scrollViewer.ScrollableWidth, null, null));

            return;
        }

        BladeClosed?.Invoke(this, blade);
        ActiveBlades.Remove(blade);

        var lastBlade = ActiveBlades.LastOrDefault();
        if (lastBlade != null && lastBlade.TitleBarVisibility == Visibility.Visible)
        {
            lastBlade.IsExpanded = true;
        }
    }

    private ScrollViewer GetScrollViewer()
    {
        return _scrollViewer ?? (_scrollViewer = this.FindDescendant<ScrollViewer>());
    }

    private void AdjustBladeItemSize()
    {
        // Adjust blade items to be full screen
        if (BladeMode == BladeMode.Fullscreen && GetScrollViewer() != null)
        {
            foreach (var item in Items)
            {
                var blade = GetBladeItem(item);
                blade.Width = _scrollViewer.ActualWidth;
                blade.Height = _scrollViewer.ActualHeight;
            }
        }
    }

    private void ItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
    {
        if (BladeMode == BladeMode.Fullscreen)
        {
            var bladeItem = GetBladeItem(sender[(int)e.Index]);
            if (bladeItem != null)
            {
                if (!_cachedBladeItemSizes.ContainsKey(bladeItem))
                {
                    // Execute change of blade item size when a blade item is added in Fullscreen mode
                    _cachedBladeItemSizes.Add(bladeItem, new Size(bladeItem.Width, bladeItem.Height));
                    AdjustBladeItemSize();
                }
            }
        }
        else if (e.CollectionChange == CollectionChange.ItemInserted)
        {
            UpdateLayout();
            GetScrollViewer()?.ChangeView(_scrollViewer.ScrollableWidth, null, null);
        }
    }
}
