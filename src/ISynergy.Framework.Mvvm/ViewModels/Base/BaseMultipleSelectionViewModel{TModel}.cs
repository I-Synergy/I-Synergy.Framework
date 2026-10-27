using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels.Base;

/// <summary>
/// Abstract base for multiple-selection ViewModels. Contains all shared selection logic
/// (items, filtering, validation, command wiring) so that Blade and Dialog variants
/// only need to provide their specific infrastructure (Owner/IsDisabled or ApplyQueryAttributes).
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public abstract class BaseMultipleSelectionViewModel<TModel> : ViewModelSelectable<List<TModel>>, IMultipleSelectionViewModel
{
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

    /// <inheritdoc />
    public virtual void SetItems(IEnumerable<object> e)
    {
        RawItems = new List<TModel>();
        Items = new ObservableCollection<TModel>();

        foreach (TModel item in e.EnsureNotNull())
        {
            RawItems.Add(item);
            Items.Add(item);
        }
    }

    List<object>? IMultipleSelectionViewModel.SelectedItem => SelectedItem?.Cast<object>().ToList();

    /// <inheritdoc />
    public virtual void SetSelectedItem(IEnumerable<object>? e)
    {
        SelectedItem = new List<TModel>();

        foreach (var item in e.EnsureNotNull())
            SelectedItem.Add((TModel)item!);
    }

    public AsyncRelayCommand<string> RefreshCommand { get; private set; }

    public AsyncRelayCommand<object> SelectCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMultipleSelectionViewModel{TEntity}"/> class.
    /// </summary>
    protected BaseMultipleSelectionViewModel(
        ICommonServices commonServices,
        ILogger<ViewModelSelectable<List<TModel>>> logger)
        : base(commonServices, logger)
    {
        RawItems = new List<TModel>();
        Items = new ObservableCollection<TModel>();
        SelectedItem = new List<TModel>();

        Validator = new Action<IObservableValidatedClass>(_ =>
        {
            if (SelectedItem is null || SelectedItem.Count < 1)
                AddValidationError(nameof(SelectedItem), _commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        RefreshCommand = new AsyncRelayCommand<string>(e => QueryItemsAsync(e));
        SelectCommand = new AsyncRelayCommand<object>(e => SelectAsync(e), _ => SelectedItem is not null && SelectedItem.Count > 0);

        IsInitialized = true;
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
                if (item!.ToString()?.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)
                    filteredList.Add(item);
            }

            Items = new ObservableCollection<TModel>();
            Items.AddRange(filteredList);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Validates and submits the selected items via <see cref="SelectCommand"/>.
    /// </summary>
    private async Task SelectAsync(object e)
    {
        if (Validate())
        {
            var result = new List<TModel>();

            foreach (TModel item in SelectedItem.EnsureNotNull())
                result.Add(item);

            OnSubmitted(new SubmitEventArgs<List<TModel>>(result));
            await CloseAsync();
        }
    }

    /// <inheritdoc />
    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;
            Items = null;
            SelectedItem = null;
            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
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
