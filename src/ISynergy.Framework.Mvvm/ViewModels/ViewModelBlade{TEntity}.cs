using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBlade.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelBlade" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelBlade" />
public abstract class ViewModelBlade<TEntity> : ViewModel, IViewModelBlade
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
    /// Gets or sets the Owner property value.
    /// </summary>
    /// <value>The owner.</value>
    public IViewModelBladeView Owner
    {
        get { return GetValue<IViewModelBladeView>(); }
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
    /// Gets or sets the IsDisabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
    public bool IsDisabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets the submit command.
    /// </summary>
    /// <value>The submit command.</value>
    public AsyncRelayCommand SubmitCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBlade{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="automaticValidation"></param>
    protected ViewModelBlade(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        Validator = new Action<IObservableClass>(arg =>
        {
            if (arg is ViewModelBlade<TEntity> vm &&
                vm.SelectedItem is IObservableClass selectedItem)
            {
            }
        });

        SelectedItem = TypeActivator.CreateInstance<TEntity>();
        IsNew = true;
        SubmitCommand = new AsyncRelayCommand(async () => await SubmitAsync(SelectedItem));
    }

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    public virtual Task SubmitAsync(TEntity e)
    {
        if (Validate())
        {
            OnSubmitted(new SubmitEventArgs<TEntity>(e));
            Close();
        }

        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        Validator = null;

        SubmitCommand?.Cancel();
        SubmitCommand = null;

        base.Dispose(disposing);
    }
}
