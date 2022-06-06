using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using System.Collections.Generic;
using System.Linq;


#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif WINDOWS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class SelectionView. This class cannot be inherited.
    /// Implements the <see cref="ISynergy.Framework.UI.Controls.View" />
    /// Implements the <see cref="ISynergy.Framework.Mvvm.Abstractions.IView" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.UI.Controls.View" />
    /// <seealso cref="ISynergy.Framework.Mvvm.Abstractions.IView" />
    public sealed partial class SelectionView : ISynergy.Framework.UI.Controls.View, IView
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
                viewModel.SelectedItem = new List<object>();

                if (viewModel.SelectionMode == SelectionModes.Single)
                {
                    viewModel.SelectedItem.Add(DataSummary.SelectedItem);
                }
                else
                {
                    foreach (var item in DataSummary.SelectedItems)
                    {
                        viewModel.SelectedItem.Add(item);
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
                if (viewModel.SelectionMode == SelectionModes.Single && viewModel.SelectedItem is not null && viewModel.SelectedItem.Count == 1)
                {
                    DataSummary.SelectedItem = viewModel.SelectedItem.Single();
                }
                else
                {
                    foreach (var item in viewModel.SelectedItem)
                    {
                        var index = DataSummary.Items.IndexOf(item);
                    }
                }
            }
        }
    }
}
