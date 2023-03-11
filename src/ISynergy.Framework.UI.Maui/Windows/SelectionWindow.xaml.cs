using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Windows;

public partial class SelectionWindow : ISelectionWindow
{
	public SelectionWindow()
	{
		InitializeComponent();
	}

    private void DataSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            viewModel.SelectedItems = new List<object>();

            if (viewModel.SelectionMode == SelectionModes.Single)
                viewModel.SelectedItems.Add(DataSummary.SelectedItem);
            else
            {
                foreach (var item in DataSummary.SelectedItems)
                {
                    viewModel.SelectedItems.Add(item);
                }
            }
        }
    }
}