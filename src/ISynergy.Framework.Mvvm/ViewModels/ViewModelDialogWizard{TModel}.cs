using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialogWizard.
/// Implements the <see cref="WizardViewModelBase{TModel}" />
/// Implements the <see cref="IViewModelDialogWizard{TModel}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
public abstract class ViewModelDialogWizard<TModel> : WizardViewModelBase<TModel>, IViewModelDialogWizard<TModel>
{
    protected ViewModelDialogWizard(
        ICommonServices commonServices,
        ILogger<ViewModelDialogWizard<TModel>> logger)
        : base(commonServices, logger)
    {
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object? result) && result is TModel entity)
            SetSelectedItem(entity);
    }

    protected override void PerformBackAction()
    {
        Validate();

        if (Page > 1)
            Page -= 1;
    }

    protected override void PerformNextAction()
    {
        Validate();

        if (Page < Pages)
            Page += 1;
    }
}
