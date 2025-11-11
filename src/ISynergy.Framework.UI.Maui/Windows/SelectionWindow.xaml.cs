using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Windows;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class SelectionWindow : ISelectionWindow, IDisposable
{
    private bool _isDisposed = false;

    public SelectionWindow()
    {
        InitializeComponent();
        BindingContextChanged += SelectionWindow_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void SelectionWindow_BindingContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            // Only initialize DataSummary selection if ViewModel has pre-selected items
            // Don't clear existing selections when BindingContext changes
            if (viewModel.SelectedItems is not null && viewModel.SelectedItems.Count > 0)
            {
                if (viewModel.SelectionMode == SelectionModes.Single && viewModel.SelectedItems.Count == 1)
                {
                    DataSummary.SelectedItem = viewModel.SelectedItems.Single();
                }
                else if (viewModel.SelectionMode == SelectionModes.Multiple)
                {
                    DataSummary.SelectedItems = new List<object>();

                    foreach (var item in viewModel.SelectedItems.EnsureNotNull())
                    {
                        DataSummary.SelectedItems.Add(item);
                    }
                }
            }
        }
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            viewModel.SelectedItems.Clear();

            if (viewModel.SelectionMode == SelectionModes.Single)
            {
                if (DataSummary.SelectedItem is not null)
                    viewModel.SelectedItems.Add(DataSummary.SelectedItem);
            }
            else
            {
                foreach (var item in DataSummary.SelectedItems.EnsureNotNull())
                {
                    if (item is not null)
                        viewModel.SelectedItems.Add(item);
                }
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
            // Detach event handlers to prevent memory leaks
            BindingContextChanged -= SelectionWindow_BindingContextChanged;
            DataSummary.SelectionChanged -= DataSummary_SelectionChanged;
        }

        _isDisposed = true;
    }

    ~SelectionWindow()
    {
        Dispose(false);
    }
}