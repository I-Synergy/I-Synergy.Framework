using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Tracks the blade view model and the blade collection whose changes a head-specific
/// <c>BladeView</c> control renders. The subscription and change-detection rules are identical on
/// every head; only the data-context property and the rendered element types differ, so they stay
/// in the control.
/// </summary>
public sealed class BladeSourceTracker : IDisposable
{
    private readonly Func<object?> _dataContextProvider;
    private readonly Func<ObservableCollection<IView>?> _itemsSourceProvider;
    private readonly Action _onBladesChanged;
    private readonly Action _onViewModelPropertyChanged;

    private IViewModelBladeView? _subscribedViewModel;
    private ObservableCollection<IView>? _subscribedCollection;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeSourceTracker"/> class.
    /// </summary>
    /// <param name="dataContextProvider">Returns the control's binding context (WPF DataContext, MAUI BindingContext).</param>
    /// <param name="itemsSourceProvider">Returns the control's explicit ItemsSource.</param>
    /// <param name="onBladesChanged">Called when the blade collection is replaced or changes.</param>
    /// <param name="onViewModelPropertyChanged">Called for any other view-model property change.</param>
    public BladeSourceTracker(
        Func<object?> dataContextProvider,
        Func<ObservableCollection<IView>?> itemsSourceProvider,
        Action onBladesChanged,
        Action onViewModelPropertyChanged)
    {
        ArgumentNullException.ThrowIfNull(dataContextProvider);
        ArgumentNullException.ThrowIfNull(itemsSourceProvider);
        ArgumentNullException.ThrowIfNull(onBladesChanged);
        ArgumentNullException.ThrowIfNull(onViewModelPropertyChanged);

        _dataContextProvider = dataContextProvider;
        _itemsSourceProvider = itemsSourceProvider;
        _onBladesChanged = onBladesChanged;
        _onViewModelPropertyChanged = onViewModelPropertyChanged;
    }

    /// <summary>
    /// Subscribes to the blade view model carried by the binding context.
    /// </summary>
    public void AttachViewModel()
    {
        if (_isDisposed)
            return;

        var viewModel = _dataContextProvider() as IViewModelBladeView;

        if (ReferenceEquals(viewModel, _subscribedViewModel))
            return;

        if (_subscribedViewModel is not null)
            _subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _subscribedViewModel = viewModel;

        if (_subscribedViewModel is not null)
            _subscribedViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    /// <summary>
    /// Resolves the collection of blades currently in effect.
    /// </summary>
    public ObservableCollection<IView>? ResolveBlades() =>
        _itemsSourceProvider() ?? (_dataContextProvider() as IViewModelBladeView)?.Blades;

    /// <summary>
    /// Subscribes to the collection of blades currently in effect.
    /// </summary>
    public void AttachCollection()
    {
        if (_isDisposed)
            return;

        var collection = ResolveBlades();

        if (ReferenceEquals(collection, _subscribedCollection))
            return;

        if (_subscribedCollection is not null)
            _subscribedCollection.CollectionChanged -= Blades_CollectionChanged;

        _subscribedCollection = collection;

        if (_subscribedCollection is not null)
            _subscribedCollection.CollectionChanged += Blades_CollectionChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isDisposed)
            return;

        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(IViewModelBladeView.Blades))
        {
            // The collection instance itself may have been replaced.
            AttachCollection();
            _onBladesChanged();
            return;
        }

        _onViewModelPropertyChanged();
    }

    private void Blades_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isDisposed)
            return;

        try
        {
            _onBladesChanged();
        }
        catch (Exception ex)
        {
            Trace.TraceError($"BladeView failed to handle a blade collection change: {ex}");
        }
    }

    /// <summary>
    /// Detaches every subscription. The control keeps ownership of its elements and view models.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

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
    }
}
