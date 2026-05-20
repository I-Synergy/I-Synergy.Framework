using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Base class for wizard ViewModels providing shared pagination state and navigation logic.
/// </summary>
/// <typeparam name="TModel">The type of the model entity.</typeparam>
public abstract class WizardViewModelBase<TModel> : ViewModelSelectable<TModel>
{
    public int Page
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public int Pages
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public bool Next_IsEnabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public bool Back_IsEnabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public bool Submit_IsEnabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public RelayCommand BackCommand { get; private set; }
    public RelayCommand NextCommand { get; private set; }

    protected WizardViewModelBase(
        ICommonServices commonServices,
        ILogger<WizardViewModelBase<TModel>> logger)
        : base(commonServices, logger)
    {
        BackCommand = new RelayCommand(PerformBackAction);
        NextCommand = new RelayCommand(PerformNextAction);

        Page = 1;
    }

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

    protected virtual void PerformBackAction()
    {
        if (Page > 1)
            Page -= 1;
    }

    protected virtual void PerformNextAction()
    {
        if (Page < Pages)
            Page += 1;
    }

    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;

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
            if (!IsInCleanup)
                Cleanup();

            BackCommand?.Dispose();
            NextCommand?.Dispose();
        }

        base.Dispose(disposing);
    }
}
