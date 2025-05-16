using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBladeView.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelBladeView" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelBladeView" />
public abstract class ViewModelBladeView<TEntity> : ViewModel, IViewModelBladeView
{
    /// <summary>
    /// Occurs when [submitted].
    /// </summary>
    public event EventHandler<SubmitEventArgs<TEntity>>? Submitted;
    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected virtual void OnSubmitted(SubmitEventArgs<TEntity> e) => Submitted?.Invoke(this, e);

    /// <summary>
    /// Gets or sets the Blades property value.
    /// </summary>
    /// <value>The blades.</value>
    [IgnoreValidation]
    public ObservableCollection<IView> Blades
    {
        get => GetValue<ObservableCollection<IView>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    /// <value>The items.</value>
    public ObservableCollection<TEntity> Items
    {
        get => GetValue<ObservableCollection<TEntity>>();
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
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    /// <value>The selected item.</value>
    public TEntity? SelectedItem
    {
        get => GetValue<TEntity>();
        set => SetValue(value);
    }


    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="e">The entity.</param>
    public virtual void SetSelectedItem(TEntity? e)
    {
        SelectedItem = e;

        if (SelectedItem is IObservableClass observableClass)
            observableClass.MarkAsClean();

        IsUpdate = true;
    }

    /// <summary>
    /// Gets or sets the IsUpdate property value.
    /// </summary>
    /// <value><c>true</c> if this instance is an update; otherwise (new), <c>false</c>.</value>
    public bool IsUpdate
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the submit command.
    /// </summary>
    /// <value>The submit command.</value>
    public AsyncRelayCommand<TEntity> SubmitCommand { get; private set; }

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
    public AsyncRelayCommand<TEntity> EditCommand { get; private set; }
    /// <summary>
    /// Gets or sets the delete command.
    /// </summary>
    /// <value>The delete command.</value>
    public AsyncRelayCommand<TEntity> DeleteCommand { get; private set; }
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
        ILogger<ViewModelBladeView<TEntity>> logger)
        : base(commonServices, logger)
    {
        Items = new ObservableCollection<TEntity>();
        Blades = new ObservableCollection<IView>();

        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<TEntity>(async (e) => await EditAsync(e.Clone()), e => e is not null);
        DeleteCommand = new AsyncRelayCommand<TEntity>(DeleteAsync, e => e is not null);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        SearchCommand = new AsyncRelayCommand<object>(SearchAsync);
        SubmitCommand = new AsyncRelayCommand<TEntity>(e => SubmitAsync(e), e => e is not null);
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
    public abstract Task EditAsync(TEntity e);

    /// <summary>
    /// delete as an asynchronous operation.
    /// </summary>
    /// <param name="e">The e.</param>
    public async Task DeleteAsync(TEntity e)
    {
        Argument.IsNotNull(e);

        string item;
        if (e!.GetType().GetProperty("Description")?.GetValue(e) is string value)
        {
            item = value;
        }
        else
        {
            item = LanguageService.Default.GetString("ThisItem");
        }

        if (await _commonServices.DialogService.ShowMessageAsync(
            string.Format(LanguageService.Default.GetString("WarningItemRemove"), item),
            LanguageService.Default.GetString("Delete"),
            MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
        {
            await RemoveAsync(e);
            await RefreshAsync();
        }
    }

    /// <summary>
    /// Removes the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public abstract Task RemoveAsync(TEntity e);
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
        Items.AddNewRange(Enumerable.Empty<TEntity>());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    public virtual Task SubmitAsync(TEntity e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
            OnSubmitted(new SubmitEventArgs<TEntity>(e));

        return Task.CompletedTask;
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            // Set flag to prevent property change notifications during cleanup
            IsInCleanup = true;

            // Clear selected item first
            SelectedItem = default(TEntity);

            Items?.Clear();
            Blades?.Clear();

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
            // Make sure cleanup is done before disposal
            if (!IsInCleanup)
            {
                Cleanup();
            }

            // Dispose and clear all commands
            AddCommand?.Dispose();
            EditCommand?.Dispose();
            DeleteCommand?.Dispose();
            RefreshCommand?.Dispose();
            SearchCommand?.Dispose();
            SubmitCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
