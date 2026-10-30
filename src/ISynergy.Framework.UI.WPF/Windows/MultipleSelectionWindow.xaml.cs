using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

/// <summary>
/// Interaction logic for SelectionWindow.xaml
/// </summary>
public partial class MultipleSelectionWindow : IMultipleSelectionWindow
{
    public MultipleSelectionWindow()
    {
        InitializeComponent();
        DataContextChanged += this.SelectionWindow_DataContextChanged;
    }

    protected override void OnClosed(EventArgs e)
    {
        DataContextChanged -= SelectionWindow_DataContextChanged;
        base.OnClosed(e);
    }

    private void SelectionWindow_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        // Multiple mode: pre-select all items in the ListBox
        if (ViewModel is IMultipleSelectionViewModel viewModel && viewModel.SelectedItem is not null)
        {
            foreach (var item in viewModel.SelectedItem)
            {
                var index = DataSummary.Items.IndexOf(item);
                if (index >= 0)
                    DataSummary.SelectedItems.Add(item);
            }
        }
    }

    private void DataSummary_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ViewModel is IMultipleSelectionViewModel viewModel)
        {
            viewModel.SetSelectedItem(DataSummary.SelectedItems.EnsureNotNull().Cast<object>());

            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        Left = Left + e.HorizontalChange;
        Top = Top + e.VerticalChange;
    }
}
