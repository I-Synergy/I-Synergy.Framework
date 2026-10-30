using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// A multiple-selection blade ViewModel. The user can pick multiple items at once.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <remarks>
/// The inheritance tree is six levels deep on purpose: <c>BaseSelectionViewModel</c> carries the
/// shared item/filter/command logic, and inlining it back into the single/multiple bases would
/// re-introduce the duplicated code that SonarCloud reports separately (copy-paste detection).
/// </remarks>
public class ViewModelMultipleSelectionBlade<TModel> : BaseMultipleSelectionViewModel<TModel>, IViewModelBlade<List<TModel>> // NOSONAR
{
    /// <inheritdoc />
    [IgnoreValidation]
    public IViewModelBladeView Owner
    {
        get => GetValue<IViewModelBladeView>();
        set => SetValue(value);
    }

    /// <inheritdoc />
    public bool IsDisabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelMultipleSelectionBlade{TEntity}"/> class.
    /// </summary>
    public ViewModelMultipleSelectionBlade(
        ICommonServices commonServices,
        ILogger<ViewModelMultipleSelectionBlade<TModel>> logger)
        : base(commonServices, logger)
    {
        SubmitCommand = new AsyncRelayCommand<List<TModel>>(
            execute: async e => await SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            base.Dispose(disposing);
    }
}
