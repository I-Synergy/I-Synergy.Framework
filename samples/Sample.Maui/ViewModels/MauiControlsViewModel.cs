using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class MauiControlsViewModel : ViewModelNavigation<object>
{
    public override string Title => "MAUI Controls";

    // Entry / Editor
    public string EntryText { get => GetValue<string>(); set => SetValue(value); }
    public string EditorText { get => GetValue<string>(); set => SetValue(value); }
    public string PasswordText { get => GetValue<string>(); set => SetValue(value); }
    public string SearchText { get => GetValue<string>(); set => SetValue(value); }

    // Picker
    public ObservableCollection<string> PickerItems { get; } = ["Option 1", "Option 2", "Option 3"];
    public string SelectedPickerItem { get => GetValue<string>(); set => SetValue(value); }

    // Date / Time
    public DateTime SelectedDate { get => GetValue<DateTime>(); set => SetValue(value); }
    public TimeSpan SelectedTime { get => GetValue<TimeSpan>(); set => SetValue(value); }

    // Toggles
    public bool IsToggled { get => GetValue<bool>(); set => SetValue(value); }
    public bool IsChecked { get => GetValue<bool>(); set => SetValue(value); }

    // Slider / Stepper
    public double SliderValue { get => GetValue<double>(); set => SetValue(value); }
    public double StepperValue { get => GetValue<double>(); set => SetValue(value); }

    // Progress
    public double ProgressValue { get => GetValue<double>(); set => SetValue(value); }

    // List
    public ObservableCollection<string> ListItems { get; } = ["Item Alpha", "Item Beta", "Item Gamma", "Item Delta", "Item Epsilon"];

    // Commands
    public AsyncRelayCommand ButtonCommand { get; }

    // Image
    public string ImageSource { get => GetValue<string>(); set => SetValue(value); }

    public MauiControlsViewModel(
        ICommonServices commonServices,
        ILogger<MauiControlsViewModel> logger)
        : base(commonServices, logger)
    {
        EntryText = "Sample entry";
        EditorText = "Sample editor text";
        PasswordText = "password";
        SearchText = string.Empty;
        SelectedPickerItem = PickerItems[0];
        SelectedDate = DateTime.Today;
        SelectedTime = TimeSpan.FromHours(12);
        IsToggled = true;
        IsChecked = false;
        SliderValue = 50;
        StepperValue = 1;
        ProgressValue = 0.6;

        ButtonCommand = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(100);
            _commonServices.BusyService?.StartBusy();
            await Task.Delay(1000);
            _commonServices.BusyService?.StopBusy();
        });
    }
}
