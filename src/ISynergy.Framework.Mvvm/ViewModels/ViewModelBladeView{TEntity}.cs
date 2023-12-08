using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
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
    public event EventHandler<SubmitEventArgs<TEntity>> Submitted;
    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected virtual void OnSubmitted(SubmitEventArgs<TEntity> e) => Submitted?.Invoke(this, e);

    /// <summary>
    /// Gets or sets the Blades property value.
    /// </summary>
    /// <value>The blades.</value>
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
    public TEntity SelectedItem
    {
        get => GetValue<TEntity>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsNew property value.
    /// </summary>
    /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
    public bool IsNew
    {
        get => GetValue<bool>();
        set => SetValue(value);
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
    public bool RefreshOnInitialization { get; }

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
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="refreshOnInitialization">if set to <c>true</c> [refresh on initialization].</param>
    /// <param name="automaticValidation"></param>
    protected ViewModelBladeView(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool refreshOnInitialization = true,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        RefreshOnInitialization = refreshOnInitialization;

        Validator = new Action<IObservableClass>(arg =>
        {
            if (arg is ViewModelBladeView<TEntity> vm &&
                vm.SelectedItem is IObservableClass selectedItem)
            {
            }
        });

        Items = new ObservableCollection<TEntity>();
        Blades = new ObservableCollection<IView>();

        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<TEntity>(async (e) => await EditAsync(e.Clone()));
        DeleteCommand = new AsyncRelayCommand<TEntity>(DeleteAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        SearchCommand = new AsyncRelayCommand<object>(SearchAsync);
        SubmitCommand = new AsyncRelayCommand<TEntity>(SubmitAsync);
    }

    /// <summary>
    /// initialize as an asynchronous operation.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task InitializeAsync()
    {
        if (!IsInitialized)
        {
            if (RefreshOnInitialization)
                IsInitialized = await RefreshAsync();
        }
    }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public virtual void SetSelectedItem(TEntity entity)
    {
        SelectedItem = entity;
        IsNew = false;
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
        string item;
        if (e.GetType().GetProperty("Description")?.GetValue(e) is string value)
        {
            item = value;
        }
        else
        {
            item = BaseCommonServices.LanguageService.GetString("ThisItem");
        }

        if (await BaseCommonServices.DialogService.ShowMessageAsync(
                            string.Format(BaseCommonServices.LanguageService.GetString("WarningItemRemove"), item),
                            BaseCommonServices.LanguageService.GetString("Delete"),
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
    public virtual Task<bool> RefreshAsync() => GetItemsAsync();

    /// <summary>
    /// The get items cancellation token
    /// </summary>
    private CancellationTokenSource GetItemsCancellationToken = new CancellationTokenSource();

    /// <summary>
    /// get items as an asynchronous operation.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public async Task<bool> GetItemsAsync()
    {
        var result = false;

        GetItemsCancellationToken.Cancel();
        GetItemsCancellationToken = new CancellationTokenSource();

        try
        {
            BaseCommonServices.BusyService.StartBusy();

            Items.AddNewRange(await RetrieveItemsAsync(GetItemsCancellationToken.Token));
            result = true;

            GetItemsCancellationToken.Token.ThrowIfCancellationRequested();
        }
        catch (TaskCanceledException) { }
        catch (OperationCanceledException) { }
        finally
        {
            BaseCommonServices.BusyService.EndBusy();
        }

        return result;
    }

    /// <summary>
    /// Retrieves the items asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
    public abstract Task<List<TEntity>> RetrieveItemsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public virtual Task SubmitAsync(TEntity e)
    {
        if (Validate())
            OnSubmitted(new SubmitEventArgs<TEntity>(e));

        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        Validator = null;

        AddCommand?.Cancel();
        AddCommand = null;
        EditCommand?.Cancel();
        EditCommand = null;
        DeleteCommand?.Cancel();
        DeleteCommand = null;
        RefreshCommand?.Cancel();
        RefreshCommand = null;
        SearchCommand?.Cancel();
        SearchCommand = null;
        SubmitCommand?.Cancel();
        SubmitCommand = null;

        base.Dispose(disposing);
    }
}
