using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    /// <summary>
    /// Class SelectionViewModel.
    /// Implements the <see name="ViewModelBlade{IList{TEntity}}" />
    /// </summary>
    /// <seealso name="ViewModelBlade{IList{TEntity}}" />
    public class SelectionViewModel : ViewModelDialog<List<object>>, ISelectionViewModel
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
        private IEnumerable<object> RawItems { get; set; }

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
        public ObservableCollection<object> Items
        {
            get { return GetValue<ObservableCollection<object>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItems property value.
        /// </summary>
        public List<object> SelectedItems
        {
            get => GetValue<List<object>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public AsyncRelayCommand<string> Search_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="items">The items.</param>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="selectionMode">The selection mode.</param>
        /// <param name="automaticValidation"></param>
        public SelectionViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            IEnumerable<object> items,
            IEnumerable<object> selectedItems,
            SelectionModes selectionMode = SelectionModes.Single,
            bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation)
        {
            if (items is null)
                items = items.EnsureNotNull();

            if (selectedItems is null)
                selectedItems = selectedItems.EnsureNotNull();

            SelectionMode = selectionMode;

            Validator = new Action<IObservableClass>(arg =>
            {
                if (SelectionMode == SelectionModes.Single && SelectedItems.Count != 1)
                {
                    Properties[nameof(SelectedItems)].Errors.Add(commonServices.LanguageService.GetString("WarningSelectItem"));
                }

                if (SelectionMode == SelectionModes.Multiple && SelectedItems.Count < 1)
                {
                    Properties[nameof(SelectedItems)].Errors.Add(commonServices.LanguageService.GetString("WarningSelectItem"));
                }
            });

            Search_Command = new AsyncRelayCommand<string>((e) => QueryItemsAsync(e));
            RawItems = items;
            Items = new ObservableCollection<object>(items);
            SelectedItems = new List<object>(selectedItems);
            IsInitialized = true;
        }

        /// <summary>
        /// Queries the items.
        /// </summary>
        /// <param name="query">Query parameter.</param>
        protected virtual Task QueryItemsAsync(string query)
        {
            if (IsInitialized && RawItems is not null && (string.IsNullOrEmpty(query) || query.Trim() == "*"))
            {
                Items = new ObservableCollection<object>(RawItems);
            }
            else
            {
                var filteredList = new List<object>();

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

                Items = new ObservableCollection<object>(filteredList);
            }

            return Task.CompletedTask;
        }

        public override Task SubmitAsync(List<object> e)
        {
            if (Validate())
            {
                OnSubmitted(new SubmitEventArgs<List<object>>(SelectedItems));
                Close();
            }

            return Task.CompletedTask;
        }
    }
}
