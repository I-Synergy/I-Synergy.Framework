using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
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
    /// Gets or sets the IsNew property value.
    /// </summary>
    /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
    public bool IsNew
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
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="automaticValidation">Validation trigger.</param>
    protected ViewModelNavigation(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        Validator = new Action<IObservableClass>(arg =>
        {
            if (arg is ViewModelNavigation<TEntity> vm &&
                vm.SelectedItem is IObservableClass selectedItem)
            {
            }
        });

        SubmitCommand = new AsyncRelayCommand<TEntity>(SubmitAsync);
    }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public virtual Task SetSelectedItemAsync(TEntity entity)
    {
        SelectedItem = entity;
        IsNew = false;
        return Task.CompletedTask;
    }

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

    /// <summary>
    /// Closes the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public override void Close()
    {
        base.Close();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) { }

    protected override void Dispose(bool disposing)
    {
        Validator = null;

        SubmitCommand?.Cancel();
        SubmitCommand = null;

        base.Dispose(disposing);
    }
}
