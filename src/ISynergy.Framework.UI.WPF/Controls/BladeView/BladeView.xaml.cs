using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// WPF visual host for the framework blade (edge pane) concept. It renders a scrim plus a
/// horizontal stack of blade views, dims and disables every blade except the newest one, and
/// anchors the stack to the physical edge resolved through <see cref="BladePlacement"/>.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IDisposable" />
public partial class BladeView : UserControl, IDisposable
{
    #region Dependency properties

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(ObservableCollection<IView>),
        typeof(BladeView),
        new PropertyMetadata(null, OnItemsSourceChanged));

    /// <summary>
    /// Identifies the <see cref="Anchor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnchorProperty = DependencyProperty.Register(
        nameof(Anchor),
        typeof(BladePaneAnchor),
        typeof(BladeView),
        new PropertyMetadata(BladePaneAnchor.Auto, OnAnchorChanged));

    /// <summary>
    /// Identifies the <see cref="DisabledOpacity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DisabledOpacityProperty = DependencyProperty.Register(
        nameof(DisabledOpacity),
        typeof(double),
        typeof(BladeView),
        new PropertyMetadata(0.75d, OnBladeStateChanged));

    /// <summary>
    /// Identifies the <see cref="BladeSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BladeSpacingProperty = DependencyProperty.Register(
        nameof(BladeSpacing),
        typeof(double),
        typeof(BladeView),
        new PropertyMetadata(10d, OnCardLayoutChanged));

    /// <summary>
    /// Identifies the <see cref="BladeMaxWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BladeMaxWidthProperty = DependencyProperty.Register(
        nameof(BladeMaxWidth),
        typeof(double),
        typeof(BladeView),
        new PropertyMetadata(520d, OnCardLayoutChanged));

    /// <summary>
    /// Identifies the <see cref="InnerPadding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.Register(
        nameof(InnerPadding),
        typeof(Thickness),
        typeof(BladeView),
        new PropertyMetadata(new Thickness(8), OnInnerPaddingChanged));

    /// <summary>
    /// Gets or sets the blades to host. When <c>null</c>, the blades of the
    /// <see cref="IViewModelBladeView"/> carried by the data context are used instead.
    /// </summary>
    /// <value>The blade views, in opening order.</value>
    public ObservableCollection<IView>? ItemsSource
    {
        get => (ObservableCollection<IView>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the logical edge the blade pane is anchored to.
    /// </summary>
    /// <value>The logical anchor; <see cref="BladePaneAnchor.Auto"/> by default.</value>
    public BladePaneAnchor Anchor
    {
        get => (BladePaneAnchor)GetValue(AnchorProperty);
        set => SetValue(AnchorProperty, value);
    }

    /// <summary>
    /// Gets or sets the opacity applied to every blade that is not the newest one.
    /// </summary>
    /// <value>The dimmed opacity; <c>0.75</c> by default.</value>
    public double DisabledOpacity
    {
        get => (double)GetValue(DisabledOpacityProperty);
        set => SetValue(DisabledOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between two adjacent blades.
    /// </summary>
    /// <value>The spacing in device-independent pixels; <c>10</c> by default.</value>
    public double BladeSpacing
    {
        get => (double)GetValue(BladeSpacingProperty);
        set => SetValue(BladeSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum width of a single blade card.
    /// </summary>
    /// <value>The maximum width; <c>520</c> by default.</value>
    public double BladeMaxWidth
    {
        get => (double)GetValue(BladeMaxWidthProperty);
        set => SetValue(BladeMaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding between the blade stack and the host edges.
    /// </summary>
    /// <value>The inner padding; <c>8</c> on every side by default.</value>
    public Thickness InnerPadding
    {
        get => (Thickness)GetValue(InnerPaddingProperty);
        set => SetValue(InnerPaddingProperty, value);
    }

    #endregion

    #region Fields

    private const double SlideDurationMilliseconds = 200d;
    private const double CardCornerRadius = 8d;

    private readonly Dictionary<IView, Border> _cards = [];
    private readonly HashSet<Border> _animatedCards = [];

    private ObservableCollection<IView>? _subscribedCollection;
    private IViewModelBladeView? _subscribedViewModel;
    private bool _isDisposed;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeView"/> class.
    /// </summary>
    public BladeView()
    {
        InitializeComponent();

        ScrimBorder.MouseLeftButtonUp += ScrimBorder_MouseLeftButtonUp;
        DataContextChanged += BladeView_DataContextChanged;
        Loaded += BladeView_Loaded;
        Unloaded += BladeView_Unloaded;

        ApplyAnchor();
        ApplyInnerPadding();
        AttachCollection();
        RebuildBlades();
    }

    #region Property changed callbacks

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
        {
            bladeView.AttachCollection();
            bladeView.RebuildBlades();
        }
    }

    private static void OnAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.ApplyAnchor();
    }

    private static void OnBladeStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.UpdateBladeStates();
    }

    private static void OnCardLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.ApplyCardLayout();
    }

    private static void OnInnerPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BladeView bladeView && !bladeView._isDisposed)
            bladeView.ApplyInnerPadding();
    }

    #endregion

    #region Data context and collection wiring

    private void BladeView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_isDisposed)
            return;

        AttachViewModel();
        AttachCollection();
        RebuildBlades();
    }

    private void BladeView_Unloaded(object sender, RoutedEventArgs e) => Cleanup();

    private void BladeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_isDisposed)
            return;

        AttachViewModel();
        AttachCollection();
        RebuildBlades();
    }

    private void AttachViewModel()
    {
        var viewModel = DataContext as IViewModelBladeView;

        if (ReferenceEquals(viewModel, _subscribedViewModel))
            return;

        if (_subscribedViewModel is not null)
            _subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _subscribedViewModel = viewModel;

        if (_subscribedViewModel is not null)
            _subscribedViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isDisposed)
            return;

        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(IViewModelBladeView.Blades))
        {
            // The collection instance itself may have been replaced.
            AttachCollection();
            RebuildBlades();
            return;
        }

        UpdateVisibility();
    }

    /// <summary>
    /// Resolves the collection of blades currently in effect.
    /// </summary>
    private ObservableCollection<IView>? ResolveBlades() =>
        ItemsSource ?? (DataContext as IViewModelBladeView)?.Blades;

    private void AttachCollection()
    {
        var collection = ResolveBlades();

        if (ReferenceEquals(collection, _subscribedCollection))
            return;

        if (_subscribedCollection is not null)
            _subscribedCollection.CollectionChanged -= Blades_CollectionChanged;

        _subscribedCollection = collection;

        if (_subscribedCollection is not null)
            _subscribedCollection.CollectionChanged += Blades_CollectionChanged;
    }

    private void Blades_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isDisposed)
            return;

        try
        {
            RebuildBlades();
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladeView failed to handle a blade collection change: {ex}");
        }
    }

    #endregion

    #region Blade cards

    private void RebuildBlades()
    {
        if (_isDisposed)
            return;

        var blades = ResolveBlades();
        var wanted = blades?.ToList() ?? [];

        // Drop the cards of blades that are no longer present.
        foreach (var stale in _cards.Keys.Where(blade => !wanted.Contains(blade)).ToList())
        {
            ReleaseCard(_cards[stale]);
            _cards.Remove(stale);
        }

        var ordered = new List<UIElement>(wanted.Count);

        foreach (var blade in wanted)
        {
            if (!_cards.TryGetValue(blade, out var card))
            {
                card = CreateCard(blade);
                _cards[blade] = card;
            }

            ordered.Add(card);
        }

        // Rebuild the child order deterministically.
        BladesPanel.Children.Clear();

        foreach (var card in ordered)
            BladesPanel.Children.Add(card);

        ApplyCardLayout();
        UpdateBladeStates();
        ApplyAnchor();
        UpdateVisibility();

        // Only blades that were just created slide in.
        foreach (var blade in wanted)
        {
            if (_cards.TryGetValue(blade, out var card) && !_animatedCards.Contains(card))
            {
                _animatedCards.Add(card);
                AnimateCard(card);
            }
        }
    }

    private Border CreateCard(IView blade)
    {
        var ambientFlowDirection = this.FlowDirection;

        // Derive the physical side here so there is a single source of truth: the card is
        // never built from a value captured before an anchor or flow-direction change.
        var isLeft = BladePlacement.IsStartSide(Anchor, ambientFlowDirection == FlowDirection.RightToLeft);

        // Host the blade page's CONTENT directly. A WPF Page cannot be added to a panel, and hosting
        // it in a Frame added an unnecessary navigation host inside the card (own journal, page
        // chrome sizing and the page's own background). Moving the content keeps the tree flat and
        // lets the card's themed surface show through.
        UIElement? bladeContent = null;
        var page = blade as Page;

        if (page is not null)
        {
            bladeContent = page.Content as UIElement;
            page.Content = null; // detach so it can be re-parented under the card
        }
        else
        {
            Trace.TraceError($"BladeView cannot host '{blade.GetType().FullName}': a WPF blade must derive from {nameof(Page)}.");
        }

        var presenter = new ContentPresenter
        {
            Content = bladeContent,
            Focusable = false,
            FlowDirection = ambientFlowDirection,
            DataContext = page?.DataContext ?? blade.ViewModel
        };

        // A ContentPresenter does not flow its DataContext into a UIElement content, so set it on
        // the moved content explicitly; otherwise every binding inside the blade page resolves to
        // nothing (empty lists, null commands) even though the view model is present.
        if (bladeContent is FrameworkElement contentElement)
            contentElement.DataContext = presenter.DataContext;

        // The page itself is no longer hosted, so carry its explicit size over to the presenter;
        // otherwise a blade page's Width/MinWidth (e.g. Width="380") is silently ignored and every
        // card falls back to whatever its content happens to measure.
        if (page is not null)
        {
            if (!double.IsNaN(page.Width))
                presenter.Width = page.Width;
            else if (page.MinWidth > 0d)
                presenter.MinWidth = page.MinWidth;

            if (!double.IsNaN(page.Height))
                presenter.Height = page.Height;
            else if (page.MinHeight > 0d)
                presenter.MinHeight = page.MinHeight;
        }

        var card = new Border
        {
            Child = presenter,
            Tag = page,
            BorderThickness = GetInnerBorderThickness(isLeft),
            CornerRadius = new CornerRadius(CardCornerRadius),
            MaxWidth = BladeMaxWidth,
            Margin = GetCardMargin(),
            Effect = new DropShadowEffect
            {
                BlurRadius = 16d,
                ShadowDepth = 2d,
                Opacity = 0.25d,
                Direction = isLeft ? 0d : 180d
            }
        };

        // Follow the host's theme tokens when it defines them (dynamic, so a theme switch re-tints
        // the card); fall back to the system brushes for consumers that don't.
        var application = Application.Current;

        if (application?.TryFindResource("AppSurfaceBrush") is Brush)
            card.SetResourceReference(Border.BackgroundProperty, "AppSurfaceBrush");
        else
            card.Background = SystemColors.WindowBrush;

        if (application?.TryFindResource("AppBorderBrush") is Brush)
            card.SetResourceReference(Border.BorderBrushProperty, "AppBorderBrush");
        else
            card.BorderBrush = SystemColors.ActiveBorderBrush;

        return card;
    }

    private void ReleaseCard(Border card)
    {
        BladesPanel.Children.Remove(card);
        _animatedCards.Remove(card);

        if (card.Child is ContentPresenter presenter)
        {
            try
            {
                var content = presenter.Content;
                presenter.Content = null;

                // Blade views are singletons while their view models are scoped and disposed with the
                // blade scope. Detach the page so a reused page can never write into a disposed view
                // model (the service attaches a fresh one on the next open).
                if (card.Tag is Page page)
                {
                    page.DataContext = null;

                    if (page.Content is null && content is UIElement element)
                        page.Content = element;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"BladeView failed to release a blade card: {ex}");
            }

            card.Child = null;
        }

        card.Tag = null;
        card.Effect = null;
    }

    private void ApplyCardLayout()
    {
        var margin = GetCardMargin();

        foreach (var card in _cards.Values)
        {
            card.Margin = margin;
            card.MaxWidth = BladeMaxWidth;
        }
    }

    private void ApplyInnerPadding() => BladesPanel.Margin = InnerPadding;

    /// <summary>
    /// Applies the newest-blade-wins state: the last blade is interactive, all earlier ones are dimmed.
    /// </summary>
    private void UpdateBladeStates()
    {
        // Dim every hosted card first: no blade stays interactive unless it is the newest one.
        // The view's own IsEnabled is kept in sync as well, because a blade page can be a reused
        // singleton: a page left disabled by an earlier session would never receive clicks again
        // even though its card is enabled.
        foreach (var blade in _cards.Keys)
        {
            var card = _cards[blade];

            card.IsEnabled = false;
            card.Opacity = DisabledOpacity;
            blade.IsEnabled = false;
        }

        var blades = ResolveBlades();

        // The newest blade is always the interactive one; do not require the card and blade counts
        // to match, otherwise a transient mismatch leaves every blade permanently disabled.
        if (blades is { Count: > 0 } && _cards.TryGetValue(blades[^1], out var newestCard))
        {
            newestCard.IsEnabled = true;
            newestCard.Opacity = 1d;
            blades[^1].IsEnabled = true;
        }
    }

    private void AnimateCard(Border card)
    {
        var isRtl = this.FlowDirection == FlowDirection.RightToLeft;
        var isLeft = BladePlacement.IsStartSide(Anchor, isRtl);
        var sign = BladePlacement.HorizontalSign(isLeft, isRtl);

        // Render transforms are never mirrored by WPF, so the direction sign is applied explicitly.
        var cardWidth = card.ActualWidth > 0d ? card.ActualWidth : BladeMaxWidth;
        var transform = new TranslateTransform(sign * cardWidth, 0d);

        card.RenderTransform = transform;
        transform.BeginAnimation(
            TranslateTransform.XProperty,
            new DoubleAnimation(transform.X, 0d, TimeSpan.FromMilliseconds(SlideDurationMilliseconds))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
    }

    private Thickness GetCardMargin() =>
        new(BladeSpacing / 2d, 0d, BladeSpacing / 2d, 0d);

    private static Thickness GetInnerBorderThickness(bool isLeft) =>
        isLeft ? new Thickness(0d, 0d, 1d, 0d) : new Thickness(1d, 0d, 0d, 0d);

    #endregion

    #region Layout and visibility

    /// <summary>
    /// Anchors the blade stack to the physical edge resolved from <see cref="Anchor"/> and the
    /// ambient flow direction.
    /// </summary>
    private void ApplyAnchor()
    {
        if (_isDisposed)
            return;

        var isRtl = this.FlowDirection == FlowDirection.RightToLeft;
        var isLeft = BladePlacement.IsStartSide(Anchor, isRtl);

        BladesPanel.HorizontalAlignment = isLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right;

        var borderThickness = GetInnerBorderThickness(isLeft);

        foreach (var card in _cards.Values)
        {
            card.BorderThickness = borderThickness;

            if (card.Effect is DropShadowEffect shadow)
                shadow.Direction = isLeft ? 0d : 180d;
        }
    }

    private void UpdateVisibility()
    {
        if (_isDisposed)
            return;

        var count = ResolveBlades()?.Count ?? 0;
        var isPaneVisible = DataContext is not IViewModelBladeView viewModel || viewModel.IsPaneVisible;

        Visibility = count > 0 && isPaneVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == FlowDirectionProperty && !_isDisposed)
        {
            // The inherited flow direction changed: re-resolve the physical anchor and mirror
            // the blade content.
            var ambientFlowDirection = this.FlowDirection;

            foreach (var card in _cards.Values)
            {
                if (card.Child is ContentPresenter presenter)
                    presenter.FlowDirection = ambientFlowDirection;
            }

            ApplyAnchor();
        }
    }

    #endregion

    #region Input

    private void ScrimBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDisposed)
            return;

        try
        {
            var blades = ResolveBlades();

            if (blades is null || blades.Count == 0)
                return;

            // Closing is delegated to the blade's view model: its Closed event drives
            // BaseBladeService.RemoveBlade, which removes the blade from the owner.
            var newest = blades[^1];

            if (newest.ViewModel is IViewModel viewModel && viewModel.CloseCommand.CanExecute(null))
                viewModel.CloseCommand.Execute(null);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladeView failed to close the newest blade: {ex}");
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Detaches every event handler and empties the blade stack. The blade views and their
    /// view models are NOT disposed: the DI scope that created them owns their lifetime.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
            Cleanup();

        _isDisposed = true;
    }

    private void Cleanup()
    {
        if (_subscribedCollection is not null)
        {
            _subscribedCollection.CollectionChanged -= Blades_CollectionChanged;
            _subscribedCollection = null;
        }

        if (_subscribedViewModel is not null)
        {
            _subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _subscribedViewModel = null;
        }

        ScrimBorder.MouseLeftButtonUp -= ScrimBorder_MouseLeftButtonUp;
        DataContextChanged -= BladeView_DataContextChanged;
        Loaded -= BladeView_Loaded;
        Unloaded -= BladeView_Unloaded;

        foreach (var card in _cards.Values)
            ReleaseCard(card);

        _cards.Clear();
        _animatedCards.Clear();
        BladesPanel.Children.Clear();
    }

    #endregion
}
