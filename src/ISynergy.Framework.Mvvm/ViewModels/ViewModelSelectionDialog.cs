using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialogSelection.
/// Implements the <see name="ViewModelDialog{List{object}}" />
/// </summary>
/// <seealso name="ViewModelDialog{List{object}}" />
public class ViewModelSelectionDialog<TEntity> : ViewModelDialog<List<TEntity>>, ISelectionViewModel
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
    [IgnoreValidation]
    private List<TEntity> RawItems { get; set; }

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
    /// Gets or sets the SelectedItems property value.
    /// </summary>
    public List<object> SelectedItems
    {
        get => GetValue<List<object>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Query property value.
    /// </summary>
    public string Query
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public AsyncRelayCommand<string> RefreshCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelSelectionDialog{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="items">The items.</param>
    /// <param name="selectedItems">The selected items.</param>
    /// <param name="selectionMode">The selection mode.</param>
    /// <param name="automaticValidation"></param>
    public ViewModelSelectionDialog(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        IEnumerable<TEntity> items,
        IEnumerable<TEntity> selectedItems,
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
            if (SelectionMode == SelectionModes.Single && SelectedItems.Count < 1)
                AddValidationError(nameof(SelectedItems), commonServices.LanguageService.GetString("WarningSelectItem"));

            if (SelectionMode == SelectionModes.Multiple && SelectedItems.Count < 1)
                AddValidationError(nameof(SelectedItems), commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        RefreshCommand = new AsyncRelayCommand<string>((e) => QueryItemsAsync(e));
        RawItems = items.ToList();

        Items = new ObservableCollection<TEntity>();
        Items.AddRange(items);

        SelectedItems = new List<object>();

        foreach (var item in selectedItems.EnsureNotNull())
        {
            SelectedItems.Add(item);
        }

        OnPropertyChanged(nameof(SelectedItems));

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
            Items = new ObservableCollection<TEntity>(RawItems);
        }
        else
        {
            var filteredList = new List<TEntity>();

            foreach (var item in RawItems.EnsureNotNull())
            {
                if (item.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    filteredList.Add(item);
                }
            }

            Items = new ObservableCollection<TEntity>();
            Items.AddRange(filteredList);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits selection
    /// </summary>
    /// <param name="e"></param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns></returns>
    public override async Task SubmitAsync(List<TEntity> e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
        {
            var result = new List<TEntity>();

            foreach (TEntity item in SelectedItems)
            {
                result.Add(item);
            }

            OnSubmitted(new SubmitEventArgs<List<TEntity>>(result));
            await CloseAsync();
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();

        // Clear collections
        //Items?.Clear();
        //SelectedItems?.Clear();

        // Dispose and clear commands
        if (RefreshCommand is IDisposable refreshCommand)
        {
            refreshCommand.Dispose();
            RefreshCommand = null;
        }

        // Reset selection state
        //SelectionMode = SelectionModes.Single;
        //Query = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Ensure cleanup is called during disposal
            Cleanup();

            base.Dispose(disposing);
        }
    }
}
