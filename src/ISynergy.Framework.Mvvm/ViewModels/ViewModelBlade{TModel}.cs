using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBlade.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IViewModelBlade" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModel" />
/// <seealso cref="IViewModelBlade" />
public abstract class ViewModelBlade<TModel> : ViewModelSelectable<TModel>, IViewModelBlade<TModel>
{
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
    /// Gets or sets the IsDisabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
    public bool IsDisabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBlade{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelBlade(
    ICommonServices commonServices,
        ILogger<ViewModelBlade<TModel>> logger)
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
    public virtual async Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true)
    {
        if (Validate(validateUnderlayingProperties))
        {
            OnSubmitted(new SubmitEventArgs<TModel>(e));
            await CloseAsync();
        }
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
