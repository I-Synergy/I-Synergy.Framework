using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels.Base;

/// <summary>
/// Abstract base for multiple-selection ViewModels. Contains the selection-specific logic on top of
/// <see cref="BaseSelectionViewModel{TModel,TSelection}"/>, so that Blade and Dialog variants only
/// need to provide their specific infrastructure (Owner/IsDisabled or ApplyQueryAttributes).
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public abstract class BaseMultipleSelectionViewModel<TModel> : BaseSelectionViewModel<TModel, List<TModel>>, IMultipleSelectionViewModel
{
    private List<object>? _selectedItemProjection;
    private List<TModel>? _selectedItemProjectionSource;

    /// <inheritdoc />
    /// <remarks>
    /// Cached per source-list reference so repeated reads do not allocate a new projection each
    /// time (S2365). The underlying list is only ever replaced, never mutated in place, so keying
    /// on reference identity is sufficient to keep the cache coherent.
    /// </remarks>
    List<object>? IMultipleSelectionViewModel.SelectedItem
    {
        get
        {
            if (!ReferenceEquals(_selectedItemProjectionSource, SelectedItem))
            {
                _selectedItemProjectionSource = SelectedItem;
                _selectedItemProjection = SelectedItem?.Cast<object>().ToList();
            }

            return _selectedItemProjection;
        }
    }

    /// <inheritdoc />
    public virtual void SetSelectedItem(IEnumerable<object>? items)
    {
        SelectedItem = new List<TModel>();

        foreach (var item in items.EnsureNotNull())
            SelectedItem.Add((TModel)item);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMultipleSelectionViewModel{TModel}"/> class.
    /// </summary>
    protected BaseMultipleSelectionViewModel(
        ICommonServices commonServices,
        ILogger<BaseMultipleSelectionViewModel<TModel>> logger)
        : base(commonServices, logger)
    {
        SelectedItem = new List<TModel>();

        Validator = new Action<IObservableValidatedClass>(_ =>
        {
            if (SelectedItem is null || SelectedItem.Count < 1)
                AddValidationError(nameof(SelectedItem), _commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        SelectCommand = new AsyncRelayCommand<object>(_ => SelectAsync(), _ => SelectedItem is not null && SelectedItem.Count > 0);

        IsInitialized = true;
    }

    /// <inheritdoc />
    protected override void OnCleanupSelection() => SelectedItem = null;

    /// <summary>
    /// Validates and submits the selected items via <see cref="BaseSelectionViewModel{TModel,TSelection}.SelectCommand"/>.
    /// </summary>
    private async Task SelectAsync()
    {
        if (Validate())
        {
            var result = new List<TModel>();

            foreach (TModel item in SelectedItem.EnsureNotNull())
                result.Add(item);

            OnSubmitted(new SubmitEventArgs<List<TModel>>(result));
            await CloseAsync();
        }
    }
}
