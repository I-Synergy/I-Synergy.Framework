using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI;

/// <summary>
/// A selection view hardcoded to multiple-selection mode.
/// </summary>
public partial class MultipleSelectionView : ISynergy.Framework.UI.Controls.View, IMultipleSelectionView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleSelectionView"/> class.
    /// </summary>
    public MultipleSelectionView()
    {
        InitializeComponent();
        DataContextChanged += MultipleSelectionView_DataContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void DataSummary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel)
        {
            viewModel.SetSelectedItems(new List<object>());

            foreach (var item in DataSummary.SelectedItems.EnsureNotNull())
            {
                viewModel.SelectedItems?.Add(item);
            }

            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    private void MultipleSelectionView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel
            && viewModel.SelectedItems is not null)
        {
            foreach (var item in viewModel.SelectedItems)
            {
                var index = DataSummary.Items.IndexOf(item);
                if (index >= 0)
                    DataSummary.SelectedItems.Add(item);
            }
        }
    }
}
