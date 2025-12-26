using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
public abstract class View : ContentPage, IView
{
    private Label? _titleLabel;
    private Microsoft.Maui.Controls.View? _originalContent;
    private bool _isWrapping = false;

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

        // Don't wrap if already wrapped or content is null
        if (content is null || content is Grid grid && grid.Children.Count > 1)
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

            // Create vertical stack layout with title and content
            VerticalStackLayout contentStackLayout = new VerticalStackLayout
            {
                Spacing = 0
            };

            contentStackLayout.Children.Add(_titleLabel);
            contentStackLayout.Children.Add(content);

            // Create the busy indicator with overlay
            BusyIndicator indicatorControl = new BusyIndicator()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                ShowOverlay = true
            };

            // Create main grid to hold content and overlay
            Grid mainGrid = new Grid();
            mainGrid.Children.Add(contentStackLayout);
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
    protected View(Type viewModelType)
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
