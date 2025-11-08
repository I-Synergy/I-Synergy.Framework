using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialog.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelDialog{TEntity}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelDialog{TEntity}" />
public abstract class ViewModelDialog<TModel> : ViewModel, IViewModelDialog<TModel>
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
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    /// <value>The selected item.</value>
    public TModel? SelectedItem
    {
        get { return GetValue<TModel>(); }
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
    /// Gets the submit command.
    /// </summary>
    /// <value>The submit command.</value>
    public AsyncRelayCommand<TModel> SubmitCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelDialog{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelDialog(
        ICommonServices commonServices,
        ILogger<ViewModelDialog<TModel>> logger)
    : base(commonServices, logger)
    {
        // CanExecute checks both parameter and property to handle MAUI Button visual state correctly
        // Execute uses parameter if provided, falls back to SelectedItem
        SubmitCommand = new AsyncRelayCommand<TModel>(
            execute: async e => await SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    public virtual async Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
        {
            OnSubmitted(new SubmitEventArgs<TModel>(e));
            await CloseAsync();
        }
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object? result) && result is TModel entity)
            SetSelectedItem(entity);
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

            // Reset dialog state
            IsUpdate = false;

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

            // Dispose and clear submit command
            SubmitCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
