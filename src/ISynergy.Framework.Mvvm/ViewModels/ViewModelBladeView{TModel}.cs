using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBladeView.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelBladeView" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelBladeView" />
public abstract class ViewModelBladeView<TModel> : ViewModel, IViewModelBladeView<TModel>
{
    /// <summary>
    /// Occurs when [submitted].
    /// </summary>
    public event EventHandler<SubmitEventArgs<TModel>>? Submitted;
    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected virtual void OnSubmitted(SubmitEventArgs<TModel> e) => Submitted?.Invoke(this, e);

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
    public ObservableCollection<TModel> Items
    {
        get => GetValue<ObservableCollection<TModel>>();
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
    public TModel? SelectedItem
    {
        get => GetValue<TModel>();
        set
        {
            var oldValue = GetValue<TModel>();

            // Unsubscribe from old item
            if (oldValue is INotifyPropertyChanged oldItem)
            {
                oldItem.PropertyChanged -= SelectedItem_PropertyChanged;
            }

            SetValue(value);

            // Subscribe to new item if it implements INotifyPropertyChanged
            if (value is INotifyPropertyChanged newItem)
            {
                newItem.PropertyChanged += SelectedItem_PropertyChanged;
            }

            // Notify command that SelectedItem changed
            SubmitCommand?.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Handles property changes within SelectedItem and propagates them.
    /// </summary>
    private void SelectedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _logger.LogTrace($"SelectedItem property '{e.PropertyName}' changed in {GetType().Name}");

        // When any property in SelectedItem changes, notify that SelectedItem itself changed
        // This ensures the UI and commands re-evaluate
        RaisePropertyChanged(nameof(SelectedItem));
        SubmitCommand?.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="isUpdate"></param>
    public virtual void SetSelectedItem(TModel e, bool isUpdate = true)
    {
        SelectedItem = e;

        if (SelectedItem is IObservableValidatedClass observableClass)
            observableClass.MarkAsClean();

        IsUpdate = isUpdate;
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
    public AsyncRelayCommand<TModel> SubmitCommand { get; private set; }

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
        Items.AddNewRange(Enumerable.Empty<TModel>());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    public virtual Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
            OnSubmitted(new SubmitEventArgs<TModel>(e));

        return Task.CompletedTask;
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            // Set flag to prevent property change notifications during cleanup
            IsInCleanup = true;

            // Unsubscribe from SelectedItem before clearing
            var currentItem = GetValue<TModel>();
            if (currentItem is INotifyPropertyChanged item)
            {
                item.PropertyChanged -= SelectedItem_PropertyChanged;
            }

            // Clear selected item first
            SelectedItem = default;

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
            // Unsubscribe before disposal
            var currentItem = GetValue<TModel>();
            if (currentItem is INotifyPropertyChanged item)
            {
                item.PropertyChanged -= SelectedItem_PropertyChanged;
            }

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
