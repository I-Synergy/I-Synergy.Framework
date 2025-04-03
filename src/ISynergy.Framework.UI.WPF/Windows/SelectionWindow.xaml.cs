using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Windows;

/// <summary>
/// Interaction logic for SelectionWindow.xaml
/// </summary>
public partial class SelectionWindow : ISelectionWindow
{
    public SelectionWindow()
    {
        InitializeComponent();
        DataContextChanged += this.SelectionWindow_DataContextChanged;
    }

    private void SelectionWindow_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            if (viewModel.SelectionMode == SelectionModes.Single && viewModel.SelectedItems is not null && viewModel.SelectedItems.Count == 1)
            {
                DataSummary.SelectedItem = viewModel.SelectedItems.Single();
            }
            else
            {
                foreach (var item in viewModel.SelectedItems.EnsureNotNull())
                {
                    _ = DataSummary.Items.IndexOf(item);
                }
            }
        }
    }

    private void DataSummary_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            viewModel.SelectedItems = new List<object>();

            if (viewModel.SelectionMode == SelectionModes.Single)
            {
                if (DataSummary.SelectedItem != null)
                    viewModel.SelectedItems.Add(DataSummary.SelectedItem);
            }
            else
            {
                foreach (var item in DataSummary.SelectedItems.EnsureNotNull())
                {
                    viewModel.SelectedItems.Add(item);
                }
            }

            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        Left = Left + e.HorizontalChange;
        Top = Top + e.VerticalChange;
    }
}
