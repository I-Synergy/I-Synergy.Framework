using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// MAUI visual host for the framework blade (edge pane) concept. It renders a scrim plus a
/// horizontal stack of blade views, dims and disables every blade except the newest one, and
/// anchors the stack to the physical edge resolved through <see cref="BladePlacement"/>.
/// </summary>
/// <remarks>
/// MAUI views are <see cref="ContentPage"/> instances and therefore cannot be children of a
/// layout. Each blade's <see cref="ContentPage.Content"/> is detached and reparented into a
/// <see cref="ContentView"/>. The page content is restored when the blade is released; the
/// blade view and its view model are never disposed here — the DI scope that created them
/// owns their lifetime.
/// </remarks>
public class BladePane : ContentView, IDisposable
{
    #region Bindable properties

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> bindable property.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(ObservableCollection<IView>),
        typeof(BladePane),
        defaultValue: null,
        propertyChanged: OnItemsSourceChanged);

    /// <summary>
    /// Identifies the <see cref="Anchor"/> bindable property.
    /// </summary>
    public static readonly BindableProperty AnchorProperty = BindableProperty.Create(
        nameof(Anchor),
        typeof(BladePaneAnchor),
        typeof(BladePane),
        defaultValue: BladePaneAnchor.Auto,
        propertyChanged: OnAnchorChanged);

    /// <summary>
    /// Identifies the <see cref="DisabledOpacity"/> bindable property.
    /// </summary>
    public static readonly BindableProperty DisabledOpacityProperty = BindableProperty.Create(
        nameof(DisabledOpacity),
        typeof(double),
        typeof(BladePane),
        defaultValue: 0.75d,
        propertyChanged: OnBladeStateChanged);

    /// <summary>
    /// Identifies the <see cref="BladeSpacing"/> bindable property.
    /// </summary>
    public static readonly BindableProperty BladeSpacingProperty = BindableProperty.Create(
        nameof(BladeSpacing),
        typeof(double),
        typeof(BladePane),
        defaultValue: 10d,
        propertyChanged: OnCardLayoutChanged);

    /// <summary>
    /// Identifies the <see cref="BladeMaxWidth"/> bindable property.
    /// </summary>
    public static readonly BindableProperty BladeMaxWidthProperty = BindableProperty.Create(
        nameof(BladeMaxWidth),
        typeof(double),
        typeof(BladePane),
        defaultValue: 520d,
        propertyChanged: OnCardLayoutChanged);

    /// <summary>
    /// Identifies the <see cref="InnerPadding"/> bindable property.
    /// </summary>
    public static readonly BindableProperty InnerPaddingProperty = BindableProperty.Create(
        nameof(InnerPadding),
        typeof(Thickness),
        typeof(BladePane),
        defaultValue: new Thickness(8d),
        propertyChanged: OnInnerPaddingChanged);

    /// <summary>
    /// Gets or sets the blades to host. When <see langword="null"/>, the blades of the
    /// <see cref="IViewModelBladeView"/> carried by the binding context are used instead.
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

    private const uint SlideDurationMilliseconds = 200u;
    private const double CardCornerRadius = 8d;

    private readonly Grid _rootGrid;
    private readonly BoxView _scrim;
    private readonly Grid _positioningRoot;
    private readonly HorizontalStackLayout _bladesPanel;

    private readonly Dictionary<IView, BladeCard> _cards = [];

    private ObservableCollection<IView>? _subscribedCollection;
    private IViewModelBladeView? _subscribedViewModel;
    private IBladePaneHost? _bladePaneHost;
    private bool _isDisposed;

    /// <summary>
    /// Per-blade record so the detached page content can be restored on release.
    /// </summary>
    private sealed class BladeCard
    {
        public required Border Card { get; init; }
        public required ContentView Host { get; init; }
        public required ContentPage Page { get; init; }
        public required Microsoft.Maui.Controls.View Content { get; init; }
        public required BoxView Separator { get; init; }
        public required Grid InnerGrid { get; init; }
        public bool Animated { get; set; }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BladePane"/> class.
    /// </summary>
    public BladePane()
    {
        // Scrim: a translucent black overlay that closes the newest blade when tapped.
        _scrim = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0.5d,
            InputTransparent = false
        };

        var scrimTap = new TapGestureRecognizer();
        scrimTap.Tapped += Scrim_Tapped;
        _scrim.GestureRecognizers.Add(scrimTap);

        // Positioning container is forced to LeftToRight on purpose: under RTL MAUI mirrors the
        // layout coordinate system, which would make HorizontalOptions.Start/End ambiguous inside
        // a mirrored container. Keeping it LTR guarantees Start/End are physically left/right.
        // The ambient flow direction is read from the pane and applied to each blade's host so the
        // blade CONTENT mirrors correctly. The container is input-transparent (without cascading to
        // its children) so taps on the empty area reach the scrim while cards stay interactive.
        _bladesPanel = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Fill,
            Spacing = BladeSpacing
        };

        _positioningRoot = new Grid
        {
            FlowDirection = FlowDirection.LeftToRight,
            InputTransparent = true,
            CascadeInputTransparent = false
        };
        _positioningRoot.Children.Add(_bladesPanel);

        _rootGrid = new Grid();
        _rootGrid.Children.Add(_scrim);
        _rootGrid.Children.Add(_positioningRoot);

        Content = _rootGrid;

        BindingContextChanged += BladePane_BindingContextChanged;
        HandlerChanged += BladePane_HandlerChanged;
        HandlerChanging += BladePane_HandlerChanging;

        ApplyAnchor();
        ApplyInnerPadding();
        ApplyCardLayout();
        AttachCollection();
        RebuildBlades();
    }

    #region Property changed callbacks

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BladePane bladePane && !bladePane._isDisposed)
        {
            bladePane.AttachCollection();
            bladePane.RebuildBlades();
        }
    }

    private static void OnAnchorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BladePane bladePane && !bladePane._isDisposed)
            bladePane.ApplyAnchor();
    }

    private static void OnBladeStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BladePane bladePane && !bladePane._isDisposed)
            bladePane.UpdateBladeStates();
    }

    private static void OnCardLayoutChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BladePane bladePane && !bladePane._isDisposed)
            bladePane.ApplyCardLayout();
    }

    private static void OnInnerPaddingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BladePane bladePane && !bladePane._isDisposed)
            bladePane.ApplyInnerPadding();
    }

    #endregion

    #region Visual-tree registration

    /// <inheritdoc />
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (_isDisposed)
            return;

        if (Parent is null)
            UnregisterHost();
        else
            RegisterHost();
    }

    private void BladePane_HandlerChanged(object? sender, EventArgs e)
    {
        if (!_isDisposed)
            RegisterHost();
    }

    private void BladePane_HandlerChanging(object? sender, HandlerChangingEventArgs e)
    {
        if (!_isDisposed && e.NewHandler is null)
            UnregisterHost();
    }

    private IBladePaneHost? ResolveHost() =>
        _bladePaneHost ??= IPlatformApplication.Current?.Services?.GetService<IBladePaneHost>();

    private void RegisterHost()
    {
        try
        {
            ResolveHost()?.Register(this);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladePane failed to register its host: {ex}");
        }
    }

    private void UnregisterHost()
    {
        try
        {
            ResolveHost()?.Unregister(this);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladePane failed to unregister its host: {ex}");
        }
    }

    #endregion

    #region Binding context and collection wiring

    private void BladePane_BindingContextChanged(object? sender, EventArgs e)
    {
        if (_isDisposed)
            return;

        AttachViewModel();
        AttachCollection();
        RebuildBlades();
    }

    private void AttachViewModel()
    {
        var viewModel = BindingContext as IViewModelBladeView;

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
        ItemsSource ?? (BindingContext as IViewModelBladeView)?.Blades;

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
            Trace.TraceError($"BladePane failed to handle a blade collection change: {ex}");
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

        var newCards = new List<BladeCard>(wanted.Count);

        foreach (var blade in wanted)
        {
            if (!_cards.TryGetValue(blade, out var card))
            {
                card = CreateCard(blade);

                if (card is null)
                    continue;

                _cards[blade] = card;
            }

            newCards.Add(card);
        }

        // Rebuild the child order deterministically.
        _bladesPanel.Children.Clear();

        foreach (var card in newCards)
            _bladesPanel.Children.Add(card.Card);

        ApplyCardLayout();
        UpdateBladeStates();
        ApplyAnchor();
        UpdateVisibility();

        // Only blades that were just created slide in.
        foreach (var card in newCards)
        {
            if (card.Animated)
                continue;

            card.Animated = true;
            SlideIn(card.Card);
        }
    }

    private BladeCard? CreateCard(IView blade)
    {
        if (blade is not ContentPage page)
        {
            Trace.TraceError($"BladePane cannot host '{blade.GetType().FullName}': a MAUI blade must derive from {nameof(ContentPage)}.");
            return null;
        }

        var content = page.Content;

        if (content is null)
        {
            Trace.TraceError($"BladePane cannot host '{blade.GetType().FullName}': the page has no content to reparent.");
            return null;
        }

        // Detach the page content so it can be re-parented into a layout-compatible host.
        page.Content = null;

        var host = new ContentView
        {
            Content = content,
            BindingContext = page.BindingContext,
            FlowDirection = FlowDirection
        };

        // A 1px separator strip replaces the per-side stroke MAUI's Border cannot express;
        // it sits on the inner edge (right when anchored left, left when anchored right).
        var isLeft = BladePlacement.IsStartSide(Anchor, FlowDirection == FlowDirection.RightToLeft);
        var separator = new BoxView
        {
            WidthRequest = 1d,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = isLeft ? LayoutOptions.End : LayoutOptions.Start,
            Color = GetStrokeColor()
        };

        var innerGrid = new Grid
        {
            // Keep the positioning of the separator physical.
            FlowDirection = FlowDirection.LeftToRight
        };
        innerGrid.Children.Add(host);
        innerGrid.Children.Add(separator);

        var card = new Border
        {
            BackgroundColor = GetCardBackgroundColor(),
            Stroke = new SolidColorBrush(GetStrokeColor()),
            StrokeThickness = 1d,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(CardCornerRadius) },
            MaximumWidthRequest = BladeMaxWidth,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Padding = new Thickness(0d),
            Content = innerGrid,
            Shadow = CreateShadow(isLeft)
        };

        return new BladeCard
        {
            Card = card,
            Host = host,
            Page = page,
            Content = content,
            Separator = separator,
            InnerGrid = innerGrid
        };
    }

    private void ReleaseCard(BladeCard card)
    {
        _bladesPanel.Children.Remove(card.Card);

        try
        {
            // Blade views are singletons while their view models are scoped and disposed with the
            // blade scope. Detach the page so a reused page can never write into a disposed view
            // model (the service attaches a fresh one on the next open).
            card.Page.BindingContext = null;

            // Restore the page content so the blade can be hosted again if re-opened.
            card.Host.Content = null;

            if (card.Page.Content is null)
                card.Page.Content = card.Content;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladePane failed to release a blade card: {ex}");
        }

        // Drop the card's content; the card itself is being discarded, so clearing its shadow is
        // unnecessary. Border.Content is non-nullable in the MAUI API, so the suppression is required.
        card.Card.Content = null!;
    }

    private void ApplyCardLayout()
    {
        if (_bladesPanel is null)
            return;

        _bladesPanel.Spacing = BladeSpacing;

        foreach (var card in _cards.Values)
            card.Card.MaximumWidthRequest = BladeMaxWidth;
    }

    private void ApplyInnerPadding()
    {
        if (_bladesPanel is not null)
            _bladesPanel.Margin = InnerPadding;
    }

    /// <summary>
    /// Applies the newest-blade-wins state: the last blade is interactive, all earlier ones are dimmed.
    /// </summary>
    private void UpdateBladeStates()
    {
        // Dim every hosted card first: no blade stays interactive unless the blade list and
        // the hosted cards agree on which one is the newest.
        foreach (var card in _cards.Values.Select(entry => entry.Card))
        {
            card.IsEnabled = false;
            card.Opacity = DisabledOpacity;
        }

        var blades = ResolveBlades();

        if (blades is null || blades.Count == 0 || blades.Count != _cards.Count)
            return;

        if (_cards.TryGetValue(blades[^1], out var newest))
        {
            newest.Card.IsEnabled = true;
            newest.Card.Opacity = 1d;
        }
    }

    private void SlideIn(Border card)
    {
        if (_isDisposed || card.Handler is null)
            return;

        var isRtl = FlowDirection == FlowDirection.RightToLeft;
        var isLeft = BladePlacement.IsStartSide(Anchor, isRtl);
        var sign = BladePlacement.HorizontalSign(isLeft, isRtl);

        // Render transforms are never mirrored, so the direction sign is applied explicitly.
        var cardWidth = card.Width > 0d ? card.Width : BladeMaxWidth;
        card.TranslationX = sign * cardWidth;

        _ = AnimateCardAsync(card);
    }

    private static async Task AnimateCardAsync(Border card)
    {
        try
        {
            await card.TranslateToAsync(0d, 0d, SlideDurationMilliseconds, Easing.CubicOut);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladePane failed to animate a blade card: {ex}");
        }
    }

    private static Shadow CreateShadow(bool isLeft) => new()
    {
        Offset = new Point(isLeft ? 3f : -3f, 2f),
        Radius = 16f,
        Opacity = 0.25f,
        // A black shadow is invisible on a dark surface; keeping the brush explicit avoids the
        // platform falling back to a light one, which reads as a glow around the card.
        Brush = Colors.Black
    };

    private static bool IsDarkTheme => Application.Current?.RequestedTheme == AppTheme.Dark;

    private static Color GetCardBackgroundColor() =>
        (IsDarkTheme
            ? FindColor("AppSurfaceDark", "TintedGray900", "AppSurface", "Gray950")
            : FindColor("AppSurfaceLight", "TintedGray100", "AppSurface", "Gray000"))
        ?? (IsDarkTheme ? Color.FromArgb("#14201C") : Colors.White);

    private static Color GetStrokeColor() =>
        (IsDarkTheme
            ? FindColor("AppBorderDark", "TintedGray600", "AppBorder", "Gray600")
            : FindColor("AppBorderLight", "TintedGray300", "AppBorder", "Gray300"))
        ?? (IsDarkTheme ? Color.FromArgb("#26352F") : Colors.Gray);

    private static Color? FindColor(params string[] keys)
    {
        foreach (var key in keys)
        {
            var color = Utilities.ResourceUtility.FindResource<Color>(key);

            if (color is not null)
                return color;
        }

        return null;
    }

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

        var isRtl = FlowDirection == FlowDirection.RightToLeft;
        var isLeft = BladePlacement.IsStartSide(Anchor, isRtl);

        _bladesPanel.HorizontalOptions = isLeft ? LayoutOptions.Start : LayoutOptions.End;

        foreach (var card in _cards.Values)
        {
            card.Separator.HorizontalOptions = isLeft ? LayoutOptions.End : LayoutOptions.Start;
            card.Host.FlowDirection = FlowDirection;
            card.Card.Shadow = CreateShadow(isLeft);
        }
    }

    private void UpdateVisibility()
    {
        if (_isDisposed)
            return;

        var count = ResolveBlades()?.Count ?? 0;
        var isPaneVisible = BindingContext is not IViewModelBladeView viewModel || viewModel.IsPaneVisible;

        IsVisible = count > 0 && isPaneVisible;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (!_isDisposed && propertyName == FlowDirectionProperty.PropertyName)
        {
            // The inherited flow direction changed: re-resolve the physical anchor and mirror
            // the blade content.
            ApplyAnchor();
        }
    }

    #endregion

    #region Input

    private void Scrim_Tapped(object? sender, TappedEventArgs e)
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
            Trace.TraceError($"BladePane failed to close the newest blade: {ex}");
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Detaches every event handler, unregisters the host and empties the blade stack. The
    /// blade views and their view models are NOT disposed: the DI scope that created them
    /// owns their lifetime.
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
        UnregisterHost();

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

        if (_scrim.GestureRecognizers.Count > 0 && _scrim.GestureRecognizers[0] is TapGestureRecognizer tap)
            tap.Tapped -= Scrim_Tapped;

        BindingContextChanged -= BladePane_BindingContextChanged;
        HandlerChanged -= BladePane_HandlerChanged;
        HandlerChanging -= BladePane_HandlerChanging;

        foreach (var card in _cards.Values)
            ReleaseCard(card);

        _cards.Clear();
        _bladesPanel.Children.Clear();
    }

    #endregion
}
