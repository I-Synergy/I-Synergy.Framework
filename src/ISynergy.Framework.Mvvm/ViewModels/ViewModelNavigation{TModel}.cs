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
/// Class ViewModelNavigation.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelNavigation{TEntity}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelNavigation{TEntity}" />
public abstract class ViewModelNavigation<TModel> : ViewModelSelectable<TModel>, IViewModelNavigation<TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelNavigation{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelNavigation(
        ICommonServices commonServices,
        ILogger<ViewModelNavigation<TModel>> logger)
        : base(commonServices, logger)
    {
        // CanExecute checks both parameter and property to handle MAUI Button visual state correctly
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
    public new virtual Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
            OnSubmitted(new SubmitEventArgs<TModel>(e));

        return Task.CompletedTask;
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
