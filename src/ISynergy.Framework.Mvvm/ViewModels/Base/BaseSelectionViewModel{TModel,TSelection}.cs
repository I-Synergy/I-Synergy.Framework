using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels.Base;

/// <summary>
/// Shared item, filtering and cleanup logic for the single- and multiple-selection ViewModels.
/// </summary>
/// <typeparam name="TModel">The model type shown in the list.</typeparam>
/// <typeparam name="TSelection">The selection type: a single model or a list of models.</typeparam>
public abstract class BaseSelectionViewModel<TModel, TSelection> : ViewModelSelectable<TSelection>, ISelectionViewModel
{
    /// <inheritdoc />
    System.Collections.IEnumerable? ISelectionViewModel.Items => Items;

    /// <inheritdoc />
    object? ISelectionViewModel.SelectedItem => SelectedItem;

    /// <inheritdoc />
    System.Windows.Input.ICommand? ISelectionViewModel.SelectCommand => SelectCommand;

    /// <inheritdoc />
    System.Windows.Input.ICommand? ISelectionViewModel.RefreshCommand => RefreshCommand;

    /// <inheritdoc/>
    public override string Title => _commonServices.LanguageService.GetString("Selection");

    private List<TModel> RawItems { get; set; }

    /// <summary>
    /// Gets or sets the items available for selection.
    /// </summary>
    public ObservableCollection<TModel>? Items
    {
        get => GetValue<ObservableCollection<TModel>?>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets the command that re-runs the item query.
    /// </summary>
    public AsyncRelayCommand<string> RefreshCommand { get; protected set; }

    /// <summary>
    /// Gets the command that validates and submits the current selection.
    /// </summary>
    public AsyncRelayCommand<object> SelectCommand { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSelectionViewModel{TModel,TSelection}"/> class.
    /// </summary>
    protected BaseSelectionViewModel(
        ICommonServices commonServices,
        ILogger<BaseSelectionViewModel<TModel, TSelection>> logger)
        : base(commonServices, logger)
    {
        RawItems = new List<TModel>();
        Items = new ObservableCollection<TModel>();

        RefreshCommand = new AsyncRelayCommand<string>(query => QueryItemsAsync(query));
        SelectCommand = new AsyncRelayCommand<object>(_ => Task.CompletedTask, _ => false);
    }

    /// <inheritdoc />
    public virtual void SetItems(IEnumerable<object> items)
    {
        RawItems = new List<TModel>();
        Items = new ObservableCollection<TModel>();

        foreach (TModel item in items.EnsureNotNull())
        {
            RawItems.Add(item);
            Items.Add(item);
        }
    }

    /// <summary>
    /// Queries/filters the items.
    /// </summary>
    protected virtual Task QueryItemsAsync(string query)
    {
        if (IsInitialized && RawItems is not null && (string.IsNullOrEmpty(query) || query.Trim() == "*"))
        {
            Items = new ObservableCollection<TModel>(RawItems);
        }
        else
        {
            var filteredList = new List<TModel>();

            foreach (var item in RawItems.EnsureNotNull())
            {
                if (item is not null && item.ToString()?.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)
                    filteredList.Add(item);
            }

            Items = new ObservableCollection<TModel>();
            Items.AddRange(filteredList);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;
            Items = null;
            OnCleanupSelection();
            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
    }

    /// <summary>
    /// Clears the platform-specific part of the selection when the ViewModel is cleaned up.
    /// </summary>
    protected virtual void OnCleanupSelection()
    {
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!IsInCleanup)
                Cleanup();

            RefreshCommand?.Dispose();
            SelectCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
