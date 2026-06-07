using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// A selection dialog ViewModel hardcoded to single-selection mode.
/// The user can pick exactly one item.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public class ViewModelSingleSelectionDialog<TModel> : ViewModelDialog<TModel>, ISingleSelectionViewModel
{
    /// <inheritdoc/>
    public override string Title { get { return _commonServices.LanguageService.GetString("Selection"); } }

    private List<TModel> RawItems { get; set; }

    /// <summary>
    /// Gets or sets the CanSubmit property value.
    /// </summary>
    public bool CanSubmit
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the items available for selection.
    /// </summary>
    public ObservableCollection<TModel>? Items
    {
        get { return GetValue<ObservableCollection<TModel>?>(); }
        set { SetValue(value); }
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

    /// <inheritdoc />
    object? ISingleSelectionViewModel.SelectedItem => SelectedItem;

    /// <inheritdoc />
    void ISingleSelectionViewModel.SetSelectedItem(object? item) => SetSelectedItem((TModel?)item);

    /// <summary>
    /// Gets or sets the Query property value.
    /// </summary>
    public string Query
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public AsyncRelayCommand<string> RefreshCommand { get; private set; }
    public AsyncRelayCommand<object> SelectCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelSingleSelectionDialog{TEntity}"/> class.
    /// </summary>
    public ViewModelSingleSelectionDialog(
        ICommonServices commonServices,
        ILogger<ViewModelSingleSelectionDialog<TModel>> logger)
        : base(commonServices, logger)
    {
        RawItems = new List<TModel>();
        Items = new ObservableCollection<TModel>();

        Validator = new Action<IObservableValidatedClass>(arg =>
        {
            if (SelectedItem == null)
                AddValidationError(nameof(SelectedItem), _commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        RefreshCommand = new AsyncRelayCommand<string>((e) => QueryItemsAsync(e));
        SelectCommand = new AsyncRelayCommand<object>((e) => SelectAsync(e), (s) => s is not null);

        IsInitialized = true;
    }

    /// <summary>
    /// Queries the items.
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
                {
                    filteredList.Add(item);
                }
            }

            Items = new ObservableCollection<TModel>();
            Items.AddRange(filteredList);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits the selected item.
    /// </summary>
    private async Task SelectAsync(object e)
    {
        if (Validate())
        {
            OnSubmitted(new SubmitEventArgs<TModel>((TModel)e));
            await CloseAsync();
        }
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;

            Items = null;

            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RefreshCommand?.Dispose();
            SelectCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
