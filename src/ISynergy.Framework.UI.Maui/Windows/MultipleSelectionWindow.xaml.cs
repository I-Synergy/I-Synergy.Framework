using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class MultipleSelectionWindow : IMultipleSelectionWindow
{
    private bool _isDisposed = false;

    public MultipleSelectionWindow()
    {
        InitializeComponent();
        BindingContextChanged += SelectionWindow_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void SelectionWindow_BindingContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel)
        {
            // Multiple mode: pre-select all items
            if (viewModel.SelectedItems is not null && viewModel.SelectedItems.Count > 0)
                DataSummary.SelectedItems = viewModel.SelectedItems.EnsureNotNull().ToList<object>();
        }
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel)
        {
            viewModel.SelectedItems?.Clear();

            foreach (var item in DataSummary.SelectedItems.EnsureNotNull().Where(item => item is not null)) // NOSONAR
            {
                viewModel.SelectedItems?.Add(item);
            }

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