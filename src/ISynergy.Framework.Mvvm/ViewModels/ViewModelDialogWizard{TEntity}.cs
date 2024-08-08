using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelDialogWizard.
/// Implements the <see cref="ViewModelDialog{TEntity}" />
/// Implements the <see cref="IViewModelDialogWizard{TEntity}" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="ViewModelDialog{TEntity}" />
/// <seealso cref="IViewModelDialogWizard{TEntity}" />
[Lifetime(Lifetimes.Scoped)]
public abstract class ViewModelDialogWizard<TEntity> : ViewModelDialog<TEntity>, IViewModelDialogWizard<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// Gets or sets the Page property value.
    /// </summary>
    /// <value>The page.</value>
    public int Page
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Pages property value.
    /// </summary>
    /// <value>The pages.</value>
    public int Pages
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Next_IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if [next is enabled]; otherwise, <c>false</c>.</value>
    public bool Next_IsEnabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Back_IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if [back is enabled]; otherwise, <c>false</c>.</value>
    public bool Back_IsEnabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Submit_IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if [submit is enabled]; otherwise, <c>false</c>.</value>
    public bool Submit_IsEnabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets the back command.
    /// </summary>
    /// <value>The back command.</value>
    public RelayCommand BackCommand { get; private set; }
    /// <summary>
    /// Gets the next command.
    /// </summary>
    /// <value>The next command.</value>
    public RelayCommand NextCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelDialogWizard{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="automaticValidation"></param>
    protected ViewModelDialogWizard(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        BackCommand = new RelayCommand(PerformBackAction);
        NextCommand = new RelayCommand(PerformNextAction);

        Page = 1;
    }

    /// <summary>
    /// Handles the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(Page))
        {
            if (Page == 1 && Page != Pages)
            {
                Back_IsEnabled = false;
                Next_IsEnabled = true;
                Submit_IsEnabled = false;
            }
            else if (Page == Pages && Page != 1)
            {
                Back_IsEnabled = true;
                Next_IsEnabled = false;
                Submit_IsEnabled = true;
            }
            else if (Page == Pages && Page == 1)
            {
                Back_IsEnabled = false;
                Next_IsEnabled = false;
                Submit_IsEnabled = true;
            }
            else
            {
                Back_IsEnabled = true;
                Next_IsEnabled = true;
                Submit_IsEnabled = false;
            }
        }
    }

    /// <summary>
    /// Performs the back action.
    /// </summary>
    private void PerformBackAction()
    {
        Validate();

        if (Page > 1)
        {
            Page -= 1;
        }
    }

    /// <summary>
    /// Performs the next action.
    /// </summary>
    private void PerformNextAction()
    {
        Validate();

        if (Page < Pages)
        {
            Page += 1;
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();

        BackCommand = null;
        NextCommand = null;
    }
}
