using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Windows;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class SelectionWindow : ISelectionWindow
{
    public SelectionWindow()
    {
        InitializeComponent();
        BindingContextChanged += SelectionWindow_BindingContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    private void SelectionWindow_BindingContextChanged(object sender, EventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            if (viewModel.SelectionMode == SelectionModes.Single && viewModel.SelectedItems is not null && viewModel.SelectedItems.Count == 1)
            {
                DataSummary.SelectedItem = viewModel.SelectedItems.Single();
            }
            else
            {
                DataSummary.SelectedItems = new List<object>();

                foreach (var item in viewModel.SelectedItems.EnsureNotNull())
                {
                    DataSummary.SelectedItems.Add(item);
                }
            }
        }
    }

    private void DataSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            viewModel.SetSelectedItems(new List<object>());

            if (viewModel.SelectionMode == SelectionModes.Single)
            {
                viewModel.SelectedItems.Add(DataSummary.SelectedItem);
            }
            else
            {
                foreach (var item in DataSummary.SelectedItems.EnsureNotNull())
                {
                    viewModel.SelectedItems.Add(item);
                }
            }
        }
    }
}