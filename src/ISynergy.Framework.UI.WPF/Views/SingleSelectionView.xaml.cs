using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using System.Windows;

namespace ISynergy.Framework.UI;

/// <summary>
/// A selection view hardcoded to single-selection mode.
/// </summary>
public partial class SingleSelectionView : ISynergy.Framework.UI.Controls.View, ISingleSelectionView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleSelectionView"/> class.
    /// </summary>
    public SingleSelectionView()
    {
        InitializeComponent();
        DataContextChanged += SingleSelectionView_DataContextChanged;
    }

    private void SingleSelectionView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel
            && viewModel.SelectedItem is not null)
        {
            DataSummary.SelectedItem = viewModel.SelectedItem;
        }
    }

    private void DataSummary_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel)
        {
            viewModel.SetSelectedItem(DataSummary.SelectedItem);
            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }
}
