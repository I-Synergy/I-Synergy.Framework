using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class View.
/// Implements the <see cref="IView" />
/// </summary>
/// <seealso cref="IView" />
[Bindable(true)]
[Lifetime(Lifetimes.Singleton)]
public abstract partial class View : Page, IView
{
    private IViewModel? _viewModel;
    private bool _isWrapped;
    private FrameworkElement? _originalContent;

    /// <summary>
    /// Gets or sets a value indicating whether the view content should be scrollable.
    /// Default is true.
    /// </summary>
    public bool Scrollable { get; set; } = true;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get
        {
            if (_viewModel is not null)
                return _viewModel;

            if (DataContext is IViewModel viewModel)
                return viewModel;

            throw new InvalidOperationException("The BindingContext is not of type IViewModel.");
        }
        set
        {
            _viewModel = value;
            DataContext = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the View class.
    /// </summary>
    protected View()
    {
        Loaded += OnViewLoaded;
    }

    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnViewLoaded;

        if (!_isWrapped && Content is not null && Scrollable)
            WrapContent();
    }

    private void WrapContent()
    {
        if (_isWrapped || Content is not FrameworkElement content)
            return;

        _originalContent = content;

        // Detach from Page before adding to ScrollViewer.
        // A FrameworkElement can only have one parent in the visual tree.
        Content = null;

        _isWrapped = true;

        var scrollViewer = new ScrollViewer
        {
            Content = _originalContent,
            VerticalScrollMode = ScrollMode.Auto,
            HorizontalScrollMode = ScrollMode.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        Content = scrollViewer;
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
#if WINDOWS
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
#else
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        base.Dispose();
    }
#endif

    // The bulk of the clean-up code is implemented in Dispose(bool)

#if IOS || MACCATALYST || ANDROID
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual new void Dispose(bool disposing)
    {
        if (disposing)
        {
            _viewModel?.Dispose();
            _viewModel = null;
        }

        base.Dispose(disposing);
    }
#else
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _viewModel is not null)
        {
            _viewModel.Dispose();
            _viewModel = null;
        }
    }
#endif
    #endregion
}
