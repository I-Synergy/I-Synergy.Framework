using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBladeWizard.
/// Implements the <see cref="WizardViewModelBase{TModel}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
public abstract class ViewModelBladeWizard<TModel> : WizardViewModelBase<TModel>, IViewModelBlade<TModel>
{
    [IgnoreValidation]
    public IViewModelBladeView Owner
    {
        get => GetValue<IViewModelBladeView>();
        set => SetValue(value);
    }

    public bool IsDisabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    protected ViewModelBladeWizard(
        ICommonServices commonServices,
        ILogger<ViewModelBladeWizard<TModel>> logger)
        : base(commonServices, logger)
    {
    }
}
