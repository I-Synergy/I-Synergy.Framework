using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Base class for ViewModels that manage a selectable item with submit/selection semantics.
/// Provides the shared Submitted event, SelectedItem with PropertyChanged subscription,
/// and the associated cleanup/dispose logic.
/// </summary>
/// <typeparam name="TModel">The type of the model entity.</typeparam>
public abstract class ViewModelSelectable<TModel> : ViewModel
{
    /// <summary>
    /// Occurs when [submitted].
    /// </summary>
    public event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected virtual void OnSubmitted(SubmitEventArgs<TModel> e)
    {
        if (Submitted != null)
        {
            var handlers = Submitted.GetInvocationList();
            foreach (EventHandler<SubmitEventArgs<TModel>> handler in handlers)
            {
                try
                {
                    handler.Invoke(this, e);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _commonServices.ExceptionHandlerService?.HandleException(ex);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Exception in Submitted handler: {ex.Message}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    public TModel? SelectedItem
    {
        get => GetValue<TModel>();
        set
        {
            var oldValue = GetValue<TModel>();

            if (oldValue is INotifyPropertyChanged oldItem)
                oldItem.PropertyChanged -= SelectedItem_PropertyChanged;

            SetValue(value);

            if (value is INotifyPropertyChanged newItem)
                newItem.PropertyChanged += SelectedItem_PropertyChanged;

            SubmitCommand?.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Handles property changes within SelectedItem and propagates them.
    /// </summary>
    protected void SelectedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _logger.LogTrace("SelectedItem property '{PropertyName}' changed in {ViewModelName}", e.PropertyName, GetType().Name);
        RaisePropertyChanged(nameof(SelectedItem));
        SubmitCommand?.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Gets or sets the IsUpdate property value.
    /// </summary>
    public bool IsUpdate
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets the submit command.
    /// </summary>
    public AsyncRelayCommand<TModel> SubmitCommand { get; protected set; }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    public virtual void SetSelectedItem(TModel? e, bool isUpdate = true)
    {
        SelectedItem = e;

        if (SelectedItem is IObservableValidatedClass observableClass)
            observableClass.MarkAsClean();

        IsUpdate = isUpdate;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelSelectable{TModel}"/> class.
    /// </summary>
    protected ViewModelSelectable(
        ICommonServices commonServices,
        ILogger<ViewModelSelectable<TModel>> logger)
        : base(commonServices, logger)
    {
        // CanExecute checks both parameter and property to handle MAUI Button visual state correctly
        // Execute uses parameter if provided, falls back to SelectedItem
        SubmitCommand = new AsyncRelayCommand<TModel>(
            execute: async e => await SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <summary>
    /// Submits the selected entity after validation.
    /// </summary>
    /// <param name="e">The entity to submit.</param>
    /// <param name="validateUnderlayingProperties">Whether to validate underlying properties.</param>
    public virtual async Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
        {
            OnSubmitted(new SubmitEventArgs<TModel>(e));
            await CloseAsync();
        }
    }

    /// <summary>
    /// Cleans up the view model, unsubscribing from SelectedItem before clearing.
    /// </summary>
    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;

            var currentItem = GetValue<TModel>();
            if (currentItem is INotifyPropertyChanged item)
                item.PropertyChanged -= SelectedItem_PropertyChanged;

            SelectedItem = default;

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
            var currentItem = GetValue<TModel>();
            if (currentItem is INotifyPropertyChanged item)
                item.PropertyChanged -= SelectedItem_PropertyChanged;

            if (!IsInCleanup)
                Cleanup();

            SubmitCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
