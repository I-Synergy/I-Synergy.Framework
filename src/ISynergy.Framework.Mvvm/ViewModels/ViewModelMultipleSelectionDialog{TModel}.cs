using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// A multiple-selection dialog ViewModel. The user can pick multiple items at once.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public class ViewModelMultipleSelectionDialog<TModel> : BaseMultipleSelectionViewModel<TModel>, IViewModelDialog<List<TModel>>
{
    /// <summary>
    /// Gets or sets the CanSubmit property value.
    /// </summary>
    public bool CanSubmit
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Query property value.
    /// </summary>
    public string Query
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelMultipleSelectionDialog{TEntity}"/> class.
    /// </summary>
    public ViewModelMultipleSelectionDialog(
        ICommonServices commonServices,
        ILogger<ViewModelMultipleSelectionDialog<TModel>> logger)
        : base(commonServices, logger)
    {
        SubmitCommand = new AsyncRelayCommand<List<TModel>>(
            execute: async e => await SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <inheritdoc />
    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object? result) && result is List<TModel> entities)
            SetSelectedItem(entities);
    }

    /// <inheritdoc />
    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;

            IsUpdate = false;

            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            base.Dispose(disposing);
    }
}
