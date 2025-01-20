using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
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

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(BladeView), new PropertyMetadata(Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"] as Brush));

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

    public static readonly DependencyProperty HorizontalBladeAlignmentProperty = DependencyProperty.Register(nameof(HorizontalBladeAlignment), typeof(HorizontalAlignment), typeof(BladeView), new PropertyMetadata(HorizontalAlignment.Right));

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

    public int AutoCollapseThreshold
    {
        get { return (int)GetValue(AutoCollapseThresholdProperty); }
        set { SetValue(AutoCollapseThresholdProperty, value); }
    }

    public static readonly DependencyProperty AutoCollapseThresholdProperty = DependencyProperty.Register(nameof(AutoCollapseThreshold), typeof(int), typeof(BladeView), new PropertyMetadata(0));

    #endregion

    private readonly Dictionary<IView, BladeState> _bladeStates = new();
    private readonly ILogger<BladeView>? _logger;
    private bool _isInternalUpdate;
    private int _viewModelHashCode;
    private bool _isDisposed;

    private class BladeState
    {
        public bool IsRemoving { get; set; }
        public Guid MessageToken { get; set; }
        public WeakReference<IViewModel> ViewModel { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public BladeView()
    {
        InitializeComponent();
        InitializeDefaultValues();
        InitializeBladeManagement();
    }

    private void InitializeDefaultValues()
    {
        this.Visibility = Visibility.Collapsed;
        this.CornerRadius = new CornerRadius(8);
        this.Padding = new Thickness(8);
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.Orientation = Orientation.Horizontal;
        this.BladeSpacing = 10;
        this.DisabledBackground = Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"] as Brush;
        this.DisabledOpacity = 0.75;
    }

    private void BladeView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Height > 0)
        {
            BladeHeight = e.NewSize.Height -
                (InnerPadding.Top + InnerPadding.Bottom + Margin.Top + Margin.Bottom);
        }
    }

    private void InitializeBladeManagement()
    {
        this.SizeChanged += BladeView_SizeChanged;
    }

    private void Blades_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        if (e.OldItems != null)
        {
            foreach (UIElement item in e.OldItems)
            {
                if (item is IView view)
                {
                    UnregisterBlade(view);
                }
            }
        }

        if (e.NewItems != null)
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

    private void RegisterBlade(IView view)
    {
        if (view.ViewModel == null) return;

        var token = Guid.NewGuid();
        _bladeStates[view] = new BladeState
        {
            MessageToken = token,
            ViewModel = new WeakReference<IViewModel>(view.ViewModel),
            RegisteredAt = DateTime.UtcNow
        };
    }

    private void UnregisterBlade(IView view)
    {
        if (_bladeStates.TryGetValue(view, out var state))
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
                bladeView.Blades.Remove(view);
            }

            if (view is IDisposable disposableView)
            {
                disposableView.Dispose();
            }

            if (state.ViewModel.TryGetTarget(out var viewModel) &&
                viewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }

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

        this.Visibility = bladeView.Blades.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        if (bladeView.Blades.Count > 0)
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
            foreach (var view in bladeView.Blades.OfType<IView>().ToList())
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

    protected void OnUnloaded(object sender, RoutedEventArgs e)
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
            CleanupBlades();
        }

        _isDisposed = true;
    }
}