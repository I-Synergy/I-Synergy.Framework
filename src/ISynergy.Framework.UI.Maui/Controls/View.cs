using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
public abstract class View : ContentPage, IView
{
    private const string WrapperMarkerValue = "__ISynergy_View_Wrapper__";
    private Label? _titleLabel;
    private ScrollView? _scrollView;
    private Grid? _contentGrid;
    private Microsoft.Maui.Controls.View? _originalContent;
    private bool _isWrapping = false;

    /// <summary>
    /// Attached property used internally to mark the wrapper Grid created by this View control.
    /// </summary>
    private static readonly BindableProperty WrapperMarkerProperty =
        BindableProperty.CreateAttached(
            "WrapperMarker",
            typeof(string),
            typeof(View),
            defaultValue: null);

    /// <summary>
    /// Gets or sets whether the title should be displayed.
    /// </summary>
    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    /// <summary>
    /// Bindable property for ShowTitle.
    /// </summary>
    public static readonly BindableProperty ShowTitleProperty =
        BindableProperty.Create(
            nameof(ShowTitle),
            typeof(bool),
            typeof(View),
            defaultValue: true,
            propertyChanged: OnShowTitleChanged);

    private static void OnShowTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is View view && view._titleLabel is not null)
        {
            view._titleLabel.IsVisible = (bool)newValue;
        }
    }

    /// <summary>
    /// Gets or sets whether the content should be scrollable.
    /// Default is true. When false, content is placed directly in the grid without a ScrollView.
    /// </summary>
    public bool Scrollable
    {
        get => (bool)GetValue(ScrollableProperty);
        set => SetValue(ScrollableProperty, value);
    }

    /// <summary>
    /// Bindable property for Scrollable.
    /// </summary>
    public static readonly BindableProperty ScrollableProperty =
        BindableProperty.Create(
            nameof(Scrollable),
            typeof(bool),
            typeof(View),
            defaultValue: true,
            propertyChanged: OnScrollableChanged);

    private static void OnScrollableChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is View view && view._contentGrid is not null && view._originalContent is not null)
        {
            var scrollable = (bool)newValue;

            if (scrollable && view._scrollView is null)
            {
                // Replace content in grid row 1 with a ScrollView
                view._contentGrid.Children.RemoveAt(1);
                view._scrollView = new ScrollView
                {
                    Content = view._originalContent,
                    Orientation = ScrollOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill
                };
                view._contentGrid.Add(view._scrollView, 0, 1);
            }
            else if (!scrollable && view._scrollView is not null)
            {
                // Replace ScrollView in grid row 1 with the content directly
                view._contentGrid.Children.RemoveAt(1);
                view._scrollView.Content = null;
                view._scrollView = null;
                view._contentGrid.Add(view._originalContent, 0, 1);
            }
        }
    }

    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get
        {
            if (BindingContext is IViewModel viewModel)
                return viewModel;

            throw new InvalidOperationException("The BindingContext is not of type IViewModel.");
        }
        set
        {
            BindingContext = value;

            // Update title label binding when ViewModel is set
            if (_titleLabel is not null)
            {
                _titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(IViewModel.Title), source: value));
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    protected View()
    {
        PropertyChanged += View_PropertyChanged;
    }

    private void View_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Content) && Content is not null && !_isWrapping)
        {
            WrapContentWithTitleAndOverlay();
        }
        else if (e.PropertyName == nameof(BindingContext) && _titleLabel is not null && BindingContext is IViewModel viewModel)
        {
            // Update title label binding when BindingContext changes
            _titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(IViewModel.Title), source: viewModel));
        }
    }

    private void WrapContentWithTitleAndOverlay()
    {
        // Store the original content
        Microsoft.Maui.Controls.View? content = Content;

        // Don't wrap if content is null
        if (content is null)
            return;

        // Check if already wrapped by looking for our specific marker
        if (content is Grid grid && grid.GetValue(WrapperMarkerProperty) as string == WrapperMarkerValue)
            return;

        _originalContent = content;

        // Set flag to prevent recursive wrapping
        _isWrapping = true;

        try
        {
            // Create title label
            _titleLabel = new Label
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(12, 12, 12, 8),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                IsVisible = ShowTitle
            };

            // Bind title label to the ViewModel's Title property
            if (BindingContext is IViewModel viewModel)
            {
                _titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(IViewModel.Title), source: viewModel));
            }

            _titleLabel.SetDynamicResource(Label.TextColorProperty, "Primary");

            // Create a grid with title row (auto) and content row (star).
            // The star row constrains the content to the remaining space.
            _contentGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star)
                }
            };

            _contentGrid.Add(_titleLabel, 0, 0);

            if (Scrollable)
            {
                // Wrap the original content in a ScrollView so it scrolls when it
                // exceeds available space instead of extending the page height.
                // Only vertical scrolling is supported.
                _scrollView = new ScrollView
                {
                    Content = content,
                    Orientation = ScrollOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill
                };
                _contentGrid.Add(_scrollView, 0, 1);
            }
            else
            {
                _contentGrid.Add(content, 0, 1);
            }

            // Create the busy indicator with overlay
            BusyIndicator indicatorControl = new BusyIndicator()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                ShowOverlay = true
            };

            // Create main grid to hold content and overlay
            Grid mainGrid = new Grid();

            // Mark this Grid as our wrapper using the attached property
            mainGrid.SetValue(WrapperMarkerProperty, WrapperMarkerValue);

            mainGrid.Children.Add(_contentGrid);
            mainGrid.Children.Add(indicatorControl);

            // Set the wrapped content
            Content = mainGrid;
        }
        finally
        {
            // Reset flag
            _isWrapping = false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="viewModelType"></param>
    protected View(Type viewModelType) // NOSONAR
    : this()
    {
        Argument.IsNotNull(viewModelType);
        ViewModel = (ServiceLocator.Default.GetRequiredService(viewModelType) as IViewModel)!;
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="viewModel"></param>
    protected View(IViewModel viewModel)
    : this()
    {
        Argument.IsNotNull(viewModel);
        ViewModel = viewModel;
    }

    /// <summary>
    /// When the page appears, automatically inherits the <see cref="FlowDirection"/>
    /// from the application's main <see cref="Microsoft.Maui.Controls.Window"/>.
    /// This ensures all runtime-created pages (dialogs, modal navigation, etc.)
    /// respect the RTL/LTR setting without per-view configuration.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Application.Current?.Windows.Count > 0)
        {
            var mainWindow = Application.Current.Windows[0];
            if (mainWindow.FlowDirection != FlowDirection.MatchParent)
            {
                FlowDirection = mainWindow.FlowDirection;
            }
        }
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from property changed
            PropertyChanged -= View_PropertyChanged;

            // free managed resources
            ViewModel?.Dispose();
        }

        // free native resources if there are any.
    }
    #endregion
}
