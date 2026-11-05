using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Base class for view models that present a selectable collection with the standard
/// add/edit/delete/refresh/search command set.
/// </summary>
/// <typeparam name="TModel">The type of the items in the collection.</typeparam>
/// <seealso cref="ViewModelSelectable{TModel}" />
public abstract class ViewModelCollection<TModel> : ViewModelSelectable<TModel>
{
    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    public ObservableCollection<TModel>? Items
    {
        get => GetValue<ObservableCollection<TModel>?>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether refreshing during initialization is disabled.
    /// </summary>
    public bool DisableRefreshOnInitialization { get; set; }

    /// <summary>
    /// Gets the add command.
    /// </summary>
    public AsyncRelayCommand AddCommand { get; private set; }

    /// <summary>
    /// Gets the edit command.
    /// </summary>
    public AsyncRelayCommand<TModel> EditCommand { get; private set; }

    /// <summary>
    /// Gets the delete command.
    /// </summary>
    public AsyncRelayCommand<TModel> DeleteCommand { get; private set; }

    /// <summary>
    /// Gets the refresh command.
    /// </summary>
    public AsyncRelayCommand RefreshCommand { get; private set; }

    /// <summary>
    /// Gets the search command.
    /// </summary>
    public AsyncRelayCommand<object> SearchCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelCollection{TModel}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger.</param>
    protected ViewModelCollection(
        ICommonServices commonServices,
        ILogger<ViewModelCollection<TModel>> logger)
        : base(commonServices, logger)
    {
        Items = new ObservableCollection<TModel>();

        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<TModel>(async (e) => await EditAsync(e.Clone()), e => e is not null);
        DeleteCommand = new AsyncRelayCommand<TModel>(RemoveAsync, e => e is not null);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        SearchCommand = new AsyncRelayCommand<object>(SearchAsync);
    }

    /// <inheritdoc />
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!IsInitialized && !DisableRefreshOnInitialization)
        {
            await RefreshAsync();
            IsInitialized = true;
        }
    }

    /// <summary>
    /// Adds an item.
    /// </summary>
    public abstract Task AddAsync();

    /// <summary>
    /// Edits the specified item.
    /// </summary>
    /// <param name="e">The item to edit.</param>
    public abstract Task EditAsync(TModel e);

    /// <summary>
    /// Removes the specified item.
    /// </summary>
    /// <param name="e">The item to remove.</param>
    public abstract Task RemoveAsync(TModel e);

    /// <summary>
    /// Searches using the specified term.
    /// </summary>
    /// <param name="e">The search term.</param>
    public abstract Task SearchAsync(object e);

    /// <summary>
    /// Refreshes the collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual Task RefreshAsync(CancellationToken cancellationToken = default) =>
        RetrieveItemsAsync(cancellationToken);

    /// <summary>
    /// Retrieves the items.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual Task RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        Items?.AddNewRange(Enumerable.Empty<TModel>());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits the specified item without closing the view model.
    /// </summary>
    /// <param name="e">The item to submit.</param>
    /// <param name="validateUnderlayingProperties">Whether to validate underlying properties.</param>
    public new virtual Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
            OnSubmitted(new SubmitEventArgs<TModel>(e));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            AddCommand?.Dispose();
            EditCommand?.Dispose();
            DeleteCommand?.Dispose();
            RefreshCommand?.Dispose();
            SearchCommand?.Dispose();
            base.Dispose(disposing);
        }
    }
}
