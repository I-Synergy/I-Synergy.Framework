using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialogSelection.
/// Implements the <see name="ViewModelDialog{List{TEntity}}" />
/// </summary>
/// <seealso name="ViewModelDialog{List{TEntity}}" />
public class ViewModelSelectionBlade<TEntity> : ViewModelBlade<List<TEntity>>, ISelectionViewModel
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
    /// Gets or sets the SelectedItems property value.
    /// </summary>
    public List<object> SelectedItems
    {
        get => GetValue<List<object>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the refresh command.
    /// </summary>
    /// <value>The refresh command.</value>
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
    public ViewModelSelectionBlade(
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
            if (SelectionMode == SelectionModes.Single && SelectedItems.Count != 1)
                AddValidationError(nameof(SelectedItems), commonServices.LanguageService.GetString("WarningSelectItem"));

            if (SelectionMode == SelectionModes.Multiple && SelectedItems.Count < 1)
                AddValidationError(nameof(SelectedItems), commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        RefreshCommand = new AsyncRelayCommand<string>((e) => QueryItemsAsync(e));
        RawItems = items;
        Items = new ObservableCollection<TEntity>(items);
        
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

            foreach (var item in RawItems)
            {
                if (item.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    filteredList.Add(item);
                }
            }

            Items = new ObservableCollection<TEntity>(filteredList);
        }

        return Task.CompletedTask;
    }

    public override Task SubmitAsync(List<TEntity> e)
    {
        if (Validate())
        {
            var result = new List<TEntity>();
            
            foreach (var item in SelectedItems.EnsureNotNull())
            {
                if (item is TEntity entity)
                    result.Add(entity);
            }

            OnSubmitted(new SubmitEventArgs<List<TEntity>>(result));
            Close();
        }

        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        Validator = null;

        RefreshCommand?.Cancel();
        RefreshCommand = null;

        base.Dispose(disposing);
    }
}
