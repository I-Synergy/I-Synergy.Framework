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
public abstract class ViewModelDialog<TModel> : ViewModelSelectable<TModel>, IViewModelDialog<TModel>
{
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
            IsInCleanup = true;

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
            base.Dispose(disposing);
        }
    }
}
