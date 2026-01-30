using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class ViewModelBladeWizard.
/// Implements the <see cref="ViewModelBlade{TEntity}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="ViewModelBlade{TEntity}" />
public abstract class ViewModelBladeWizard<TModel> : ViewModelBlade<TModel>
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
    /// Initializes a new instance of the <see cref="ViewModelBladeWizard{TEntity}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected ViewModelBladeWizard(
        ICommonServices commonServices,
        ILogger<ViewModelBladeWizard<TModel>> logger)
        : base(commonServices, logger)
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
    public override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(Page) || e.PropertyName == nameof(Pages))
        {
            Back_IsEnabled = Page > 1;
            Next_IsEnabled = Page < Pages;
            Submit_IsEnabled = Pages > 0 && Page == Pages;
        }
    }

    /// <summary>
    /// Performs the back action.
    /// </summary>
    private void PerformBackAction()
    {
        if (Page > 1)
            Page -= 1;
    }

    /// <summary>
    /// Performs the next action.
    /// </summary>
    private void PerformNextAction()
    {
        if (Page < Pages)
            Page += 1;
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            // Set flag to prevent property change notifications during cleanup
            IsInCleanup = true;

            // Reset wizard state
            Page = 1;
            Pages = 0;
            Next_IsEnabled = false;
            Back_IsEnabled = false;
            Submit_IsEnabled = false;

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
            // Make sure cleanup is done before disposal
            if (!IsInCleanup)
            {
                Cleanup();
            }

            // Dispose and clear navigation commands
            BackCommand?.Dispose();
            NextCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
