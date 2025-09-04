using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelNavigation.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelNavigation{TEntity}" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelNavigation{TEntity}" />
public abstract class ViewModelNavigation<TEntity> : ViewModel, IViewModelNavigation<TEntity>
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
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    /// <value>The selected item.</value>
    public TEntity? SelectedItem
    {
        get { return GetValue<TEntity>(); }
        set { SetValue(value); }
    }


    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="e">The entity.</param>
    public virtual void SetSelectedItem(TEntity? e)
    {
        SelectedItem = e;

        if (SelectedItem is IObservableValidatedClass observableClass)
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
    /// Initializes a new instance of the <see cref="ViewModelNavigation{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelNavigation(
        ICommonServices commonServices,
        ILogger<ViewModelNavigation<TEntity>> logger)
        : base(commonServices, logger)
    {
        SubmitCommand = new AsyncRelayCommand<TEntity>(async e => await SubmitAsync(e), e => e is not null);
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

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object? result) && result is TEntity entity)
            SetSelectedItem(entity);
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            // Set flag to prevent property change notifications during cleanup
            IsInCleanup = true;

            // Clear selected item first
            SelectedItem = default(TEntity);

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

            // Clear commands
            SubmitCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
