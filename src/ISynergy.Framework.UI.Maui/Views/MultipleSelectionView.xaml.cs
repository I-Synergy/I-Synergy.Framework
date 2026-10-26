using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;

namespace ISynergy.Framework.UI;

/// <summary>
/// A selection view hardcoded to multiple-selection mode.
/// </summary>
public partial class MultipleSelectionView : ISynergy.Framework.UI.Controls.View, IMultipleSelectionView
{
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleSelectionView"/> class.
    /// </summary>
    public MultipleSelectionView()
    {
        InitializeComponent();
        BindingContextChanged += MultipleSelectionView_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void MultipleSelectionView_BindingContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel
            && viewModel.SelectedItem is not null
            && viewModel.SelectedItem.Count > 0)
        {
            DataSummary.SelectedItems = viewModel.SelectedItem.EnsureNotNull().ToList<object>();
        }
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel)
        {
            viewModel.SetSelectedItem(DataSummary.SelectedItems.EnsureNotNull());
            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            BindingContextChanged -= MultipleSelectionView_BindingContextChanged;
            DataSummary.SelectionChanged -= DataSummary_SelectionChanged;
        }

        _isDisposed = true;
        base.Dispose(disposing);
    }
}
