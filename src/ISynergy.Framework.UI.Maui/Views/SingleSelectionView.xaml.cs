using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;

namespace ISynergy.Framework.UI;

/// <summary>
/// A selection view hardcoded to single-selection mode.
/// </summary>
public partial class SingleSelectionView : ISynergy.Framework.UI.Controls.View, ISingleSelectionView
{
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleSelectionView"/> class.
    /// </summary>
    public SingleSelectionView()
    {
        InitializeComponent();
        BindingContextChanged += SingleSelectionView_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void SingleSelectionView_BindingContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel
            && viewModel.SelectedItem is not null)
        {
            DataSummary.SelectedItem = viewModel.SelectedItem;
        }
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel)
        {
            viewModel.SetSelectedItem(DataSummary.SelectedItem);
            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            BindingContextChanged -= SingleSelectionView_BindingContextChanged;
            DataSummary.SelectionChanged -= DataSummary_SelectionChanged;
        }

        _isDisposed = true;
        base.Dispose(disposing);
    }
}
