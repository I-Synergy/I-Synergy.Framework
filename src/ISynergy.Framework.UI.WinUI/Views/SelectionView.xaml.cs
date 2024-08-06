using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class SelectionView. This class cannot be inherited.
/// Implements the <see cref="ISynergy.Framework.UI.Controls.View" />
/// Implements the <see cref="ISynergy.Framework.Mvvm.Abstractions.IView" />
/// </summary>
/// <seealso cref="ISynergy.Framework.UI.Controls.View" />
/// <seealso cref="ISynergy.Framework.Mvvm.Abstractions.IView" />
public partial class SelectionView : ISynergy.Framework.UI.Controls.View, ISelectionView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionView" /> class.
    /// </summary>
    public SelectionView()
    {
        InitializeComponent();
        DataContextChanged += SelectionView_DataContextChanged;
        DataSummary.SelectionChanged += DataSummary_SelectionChanged;
    }

    /// <summary>
    /// Datas the summary selection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    private void DataSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is ISelectionViewModel viewModel)
        {
            viewModel.SelectedItems = new List<object>();

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

    /// <summary>
    /// Selections the view data context changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="DataContextChangedEventArgs"/> instance containing the event data.</param>
    private void SelectionView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
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
}