using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels.Base;

/// <summary>
/// Abstract base for single-selection ViewModels. Contains the selection-specific logic on top of
/// <see cref="BaseSelectionViewModel{TModel,TSelection}"/>, so that Blade and Dialog variants only
/// need to provide their specific infrastructure (Owner/IsDisabled or ApplyQueryAttributes).
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public abstract class BaseSingleSelectionViewModel<TModel> : BaseSelectionViewModel<TModel, TModel>, ISingleSelectionViewModel
{
    /// <inheritdoc />
    object? ISingleSelectionViewModel.SelectedItem => SelectedItem;

    /// <inheritdoc />
    void ISingleSelectionViewModel.SetSelectedItem(object? item) => SetSelectedItem((TModel?)item);

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSingleSelectionViewModel{TModel}"/> class.
    /// </summary>
    protected BaseSingleSelectionViewModel(
        ICommonServices commonServices,
        ILogger<BaseSingleSelectionViewModel<TModel>> logger)
        : base(commonServices, logger)
    {
        Validator = new Action<IObservableValidatedClass>(_ =>
        {
            if (SelectedItem is null)
                AddValidationError(nameof(SelectedItem), _commonServices.LanguageService.GetString("WarningSelectItem"));
        });

        RefreshCommand = new AsyncRelayCommand<string>(e => QueryItemsAsync(e));
        SelectCommand = new AsyncRelayCommand<object>(e => SelectAsync(e), s => s is not null);

        IsInitialized = true;
    }

    /// <summary>
    /// Validates and submits the selected item via <see cref="BaseSelectionViewModel{TModel,TSelection}.SelectCommand"/>.
    /// </summary>
    private async Task SelectAsync(object e)
    {
        if (Validate())
        {
            OnSubmitted(new SubmitEventArgs<TModel>((TModel)e));
            await CloseAsync();
        }
    }
}
