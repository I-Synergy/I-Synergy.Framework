using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#elif (__WINUI__)
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
    public sealed partial class SelectionView : IView
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
            if (DataContext is ISelectionViewModel viewModel)
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

#if __UWP__ || (__WINUI__)
        /// <summary>
        /// Selections the view data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DataContextChangedEventArgs"/> instance containing the event data.</param>
        private void SelectionView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
#else
        private void SelectionView_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
#endif
        {
            if (DataContext is ISelectionViewModel viewModel)
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

                    //foreach (ItemIndexRange item in viewModel.SelectedItems.EnsureNotNull())
                    //{
                    //    DataSummary.SelectRange(item);
                    //};
                }
            }
        }
    }
}
