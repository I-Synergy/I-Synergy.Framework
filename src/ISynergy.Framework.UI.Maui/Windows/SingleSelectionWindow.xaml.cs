using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

public partial class SingleSelectionWindow : ISingleSelectionWindow
{
    private bool _isDisposed = false;

    public SingleSelectionWindow()
    {
        InitializeComponent();
        BindingContextChanged += SelectionWindow_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void SelectionWindow_BindingContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel)
        {
            // Single mode: only pre-select the first item, even if more were passed
            if (viewModel.SelectedItem is not null)
                DataSummary.SelectedItem = viewModel.SelectedItem;
        }
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel)
        {
            if (DataSummary.SelectedItem is not null)
                viewModel.SetSelectedItem(DataSummary.SelectedItem);

            // Force UI refresh by triggering NotifyCanExecuteChanged
            // This updates the visual state of the button
            viewModel.SelectCommand?.NotifyCanExecuteChanged();

            // Additional UI thread flush to ensure visual states update immediately
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Give the UI a chance to process the command state change
            });
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            // Detach event handlers to prevent memory leaks
            BindingContextChanged -= SelectionWindow_BindingContextChanged;
            DataSummary.SelectionChanged -= DataSummary_SelectionChanged;
        }

        _isDisposed = true;
        base.Dispose(disposing);
    }
}