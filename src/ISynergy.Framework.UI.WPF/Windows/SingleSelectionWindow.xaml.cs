using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

/// <summary>
/// Interaction logic for SelectionWindow.xaml
/// </summary>
public partial class SingleSelectionWindow : ISingleSelectionWindow
{
    public SingleSelectionWindow()
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
        // Single mode: only pre-select the first item, even if more were passed
        if (ViewModel is ISingleSelectionViewModel viewModel && viewModel.SelectedItem is not null)
            DataSummary.SelectedItem = viewModel.SelectedItem;
    }

    private void DataSummary_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ViewModel is ISingleSelectionViewModel viewModel)
        {
            if (DataSummary.SelectedItem is not null)
                viewModel.SetSelectedItem(DataSummary.SelectedItem);

            viewModel.SelectCommand.NotifyCanExecuteChanged();
        }
    }

    private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        Left = Left + e.HorizontalChange;
        Top = Top + e.VerticalChange;
    }
}
