using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialog.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelDialog{TEntity}" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelDialog{TEntity}" />
public abstract class ViewModelDialog<TEntity> : ViewModel, IViewModelDialog<TEntity>
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
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    /// <value>The selected item.</value>
    public TEntity SelectedItem
    {
        get { return GetValue<TEntity>(); }
        set { SetValue(value); }
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
    public AsyncRelayCommand<TEntity> SubmitCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelDialog{TEntity}"/> class.
    /// </summary>
    /// <param name="scopedContextService">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="automaticValidation"></param>
    protected ViewModelDialog(
        IScopedContextService scopedContextService,
        ICommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(scopedContextService, commonServices, logger, automaticValidation)
    {
        SubmitCommand = new AsyncRelayCommand<TEntity>(async e => await SubmitAsync(e));
    }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="e">The entity.</param>
    public virtual void SetSelectedItem(TEntity e)
    {
        SelectedItem = e;
        IsUpdate = true;
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    public virtual async Task SubmitAsync(TEntity e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
        {
            OnSubmitted(new SubmitEventArgs<TEntity>(e));
            await CloseAsync();
        }
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object result) && result is TEntity entity)
            SetSelectedItem(entity);
    }

    public override void Cleanup()
    {
        base.Cleanup();

        // Clear the selected item
        SelectedItem = default(TEntity);

        // Reset dialog state
        IsUpdate = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose and clear submit command
            if (SubmitCommand is IDisposable submitCommand)
            {
                submitCommand.Dispose();
                SubmitCommand = null;
            }

            base.Dispose(disposing);
        }
    }
}
