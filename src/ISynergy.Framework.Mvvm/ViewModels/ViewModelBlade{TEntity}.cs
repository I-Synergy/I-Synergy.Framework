﻿using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;

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
    /// Gets or sets the Owner property value.
    /// </summary>
    /// <value>The owner.</value>
    [IgnoreValidation]
    public IViewModelBladeView Owner
    {
        get { return GetValue<IViewModelBladeView>(); }
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
    public virtual AsyncRelayCommand<TEntity> SubmitCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBlade{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="automaticValidation"></param>
    protected ViewModelBlade(
        ICommonServices commonServices,
        bool automaticValidation = false)
        : base(commonServices, automaticValidation)
    {
        SubmitCommand = new AsyncRelayCommand<TEntity>(async e => await SubmitAsync(e), (e) => e is not null);
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

    public override void Cleanup()
    {
        try
        {
            // Set flag to prevent property change notifications during cleanup
            IsInCleanup = true;

            // Clear selected item first
            SelectedItem = default;

            base.Cleanup();
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

            // Dispose and clear the submit command
            SubmitCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
