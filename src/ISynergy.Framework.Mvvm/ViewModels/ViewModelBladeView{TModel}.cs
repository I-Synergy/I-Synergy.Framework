using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBladeView.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelBladeView" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelBladeView" />
public abstract class ViewModelBladeView<TModel> : ViewModelSelectable<TModel>, IViewModelBladeView<TModel>
{
    /// <summary>
    /// Gets or sets the Blades property value.
    /// </summary>
    /// <value>The blades.</value>
    [IgnoreValidation]
    public ObservableCollection<IView>? Blades
    {
        get => GetValue<ObservableCollection<IView>?>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    /// <value>The items.</value>
    public ObservableCollection<TModel>? Items
    {
        get => GetValue<ObservableCollection<TModel>?>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsPaneEnabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is pane enabled; otherwise, <c>false</c>.</value>
    public bool IsPaneVisible
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether [refresh on initialization].
    /// </summary>
    /// <value><c>true</c> if [refresh on initialization]; otherwise, <c>false</c>.</value>
    public bool DisableRefreshOnInitialization { get; set; }

    /// <summary>
    /// Gets or sets the add command.
    /// </summary>
    /// <value>The add command.</value>
    public AsyncRelayCommand AddCommand { get; private set; }
    /// <summary>
    /// Gets or sets the edit command.
    /// </summary>
    /// <value>The edit command.</value>
    public AsyncRelayCommand<TModel> EditCommand { get; private set; }
    /// <summary>
    /// Gets or sets the delete command.
    /// </summary>
    /// <value>The delete command.</value>
    public AsyncRelayCommand<TModel> DeleteCommand { get; private set; }
    /// <summary>
    /// Gets or sets the refresh command.
    /// </summary>
    /// <value>The refresh command.</value>
    public AsyncRelayCommand RefreshCommand { get; private set; }
    /// <summary>
    /// Gets or sets the search command.
    /// </summary>
    /// <value>The search command.</value>
    public AsyncRelayCommand<object> SearchCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBladeView{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelBladeView(
        ICommonServices commonServices,
        ILogger<ViewModelBladeView<TModel>> logger)
        : base(commonServices, logger)
    {
        Items = new ObservableCollection<TModel>();
        Blades = new ObservableCollection<IView>();

        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<TModel>(async (e) => await EditAsync(e.Clone()), e => e is not null);
        DeleteCommand = new AsyncRelayCommand<TModel>(RemoveAsync, e => e is not null);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        SearchCommand = new AsyncRelayCommand<object>(SearchAsync);
        // CanExecute checks both parameter and property to handle MAUI Button visual state correctly
        SubmitCommand = new AsyncRelayCommand<TModel>(
            execute: e => SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <summary>
    /// initialize as an asynchronous operation.
    /// </summary>
    /// <returns>Task.</returns>
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
    /// Adds the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract Task AddAsync();

    /// <summary>
    /// Edits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public abstract Task EditAsync(TModel e);

    /// <summary>
    /// Removes the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public abstract Task RemoveAsync(TModel e);

    /// <summary>
    /// Searches the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public abstract Task SearchAsync(object e);

    /// <summary>
    /// Refreshes the asynchronous.
    /// </summary>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task RefreshAsync(CancellationToken cancellationToken = default) => RetrieveItemsAsync(cancellationToken);

    /// <summary>
    /// Retrieves the items asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
    public virtual Task RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        Items?.AddNewRange(Enumerable.Empty<TModel>());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    public new virtual Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
            OnSubmitted(new SubmitEventArgs<TModel>(e));

        return Task.CompletedTask;
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;
            Items = null;
            Blades = null;
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
            AddCommand?.Dispose();
            EditCommand?.Dispose();
            DeleteCommand?.Dispose();
            RefreshCommand?.Dispose();
            SearchCommand?.Dispose();
            base.Dispose(disposing);
        }
    }
}
