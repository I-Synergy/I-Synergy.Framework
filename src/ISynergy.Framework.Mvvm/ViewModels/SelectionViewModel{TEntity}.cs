using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    /// <summary>
    /// Class SelectionViewModel.
    /// Implements the <see name="ViewModelBlade{IList{TEntity}}" />
    /// Implements the <see cref="IViewModelBlade" />
    /// </summary>
    /// <seealso name="ViewModelBlade{IList{TEntity}}" />
    /// <seealso cref="IViewModelBlade" />
    public class SelectionViewModel<TEntity> : ViewModelBlade<List<object>>, IViewModelBlade, ISelectionViewModel
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Selection"); } }

        /// <summary>
        /// Gets or sets the raw items.
        /// </summary>
        /// <value>The raw items.</value>
        private IEnumerable<TEntity> RawItems { get; set; }

        /// <summary>
        /// Gets or sets the SelectionMode property value.
        /// </summary>
        /// <value>The selection mode.</value>
        public SelectionModes SelectionMode
        {
            get { return GetValue<SelectionModes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Item property value.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<TEntity> Items
        {
            get { return GetValue<ObservableCollection<TEntity>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public Command<string> Search_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionViewModel{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="items">The items.</param>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="selectionMode">The selection mode.</param>
        public SelectionViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            IEnumerable<TEntity> items,
            IEnumerable<object> selectedItems,
            SelectionModes selectionMode = SelectionModes.Single)
            : base(context, commonServices, loggerFactory)
        {
            if (items is null)
                items = items.EnsureNotNull();

            if (selectedItems is null)
                selectedItems = selectedItems.EnsureNotNull();

            SelectionMode = selectionMode;

            Validator = new Action<IObservableClass>(arg =>
            {
                if (SelectionMode == SelectionModes.Single && SelectedItem.Count != 1)
                {
                    Properties[nameof(SelectedItem)].Errors.Add(commonServices.LanguageService.GetString("WarningSelectItem"));
                }

                if (SelectionMode == SelectionModes.Multiple && SelectedItem.Count < 1)
                {
                    Properties[nameof(SelectedItem)].Errors.Add(commonServices.LanguageService.GetString("WarningSelectItem"));
                }
            });

            Search_Command = new Command<string>(async (e) => await QueryItemsAsync(e));
            RawItems = items;
            Items = new ObservableCollection<TEntity>(items);
            SelectedItem = new List<object>(selectedItems);
            IsInitialized = true;
        }

        /// <summary>
        /// Queries the items.
        /// </summary>
        /// <param name="query">Query parameter.</param>
        protected virtual Task QueryItemsAsync(string query)
        {
            if (IsInitialized && RawItems != null)
            {
                if (string.IsNullOrEmpty(query) || query.Trim() == "*")
                {
                    Items = new ObservableCollection<TEntity>(RawItems);
                }
                else
                {
                    var filteredList = new List<TEntity>();

                    foreach (var item in RawItems)
                    {
                        foreach (var prop in item.GetType().GetProperties().Select(s => s.GetValue(item)).ToList())
                        {
                            if (prop is string value && !string.IsNullOrEmpty(value) && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                filteredList.Add(item);
                                break;
                            }
                        }
                    }

                    Items = new ObservableCollection<TEntity>(filteredList);
                }
            }

            return Task.CompletedTask;
        }
    }
}
