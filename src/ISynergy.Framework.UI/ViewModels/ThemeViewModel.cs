using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.ViewModels;

public class ColorOption : ObservableClass
{
    public string Color { get; }

    public bool IsSelected
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public ColorOption(string color) => Color = color;
}

public class ThemeViewModel : ViewModelDialog<ThemeStyle>
{
    public override string Title => _commonServices.LanguageService.GetString("Theme");

    public ObservableCollection<ColorOption> ColorOptions
    {
        get => GetValue<ObservableCollection<ColorOption>>();
        private set => SetValue(value);
    }

    public ColorOption? SelectedColorOption
    {
        get => GetValue<ColorOption?>();
        set
        {
            if (value is null) return;
            foreach (var opt in ColorOptions)
                opt.IsSelected = false;
            value.IsSelected = true;
            if (SelectedItem is not null)
                SelectedItem.Color = value.Color;
            SetValue(value);
        }
    }

    public RelayCommand<ColorOption> SelectColorCommand { get; }

    public ThemeViewModel(
        ICommonServices commonServices,
        ISettingsService settingsService,
        IOptions<ThemeColorOptions> themeColorOptions,
        ILogger<ThemeViewModel> logger)
        : base(commonServices, logger)
    {
        SelectedItem = new ThemeStyle
        {
            Theme = settingsService.LocalSettings.Theme,
            Color = settingsService.LocalSettings.Color
        };

        var availableColors = themeColorOptions.Value.AvailableColors;
        var colorStrings = availableColors.Count > 0
            ? (IEnumerable<string>)availableColors
            : ThemeColors.Colors;

        ColorOptions = new ObservableCollection<ColorOption>(
            colorStrings.Select(c => new ColorOption(c)));

        SelectColorCommand = new RelayCommand<ColorOption>(item =>
        {
            SelectedColorOption = item;
        });

        ApplySelection(SelectedItem.Color);
    }

    private void ApplySelection(string? color)
    {
        var matched = ColorOptions.FirstOrDefault(
            o => string.Equals(o.Color, color, StringComparison.OrdinalIgnoreCase));

        SelectedColorOption = matched ?? (ColorOptions.Count > 0 ? ColorOptions[0] : null);
    }

    public override void SetSelectedItem(ThemeStyle? e, bool isUpdate = true)
    {
        if (e is not null && ColorOptions.Count > 0)
        {
            var matched = ColorOptions.FirstOrDefault(
                o => string.Equals(o.Color, e.Color, StringComparison.OrdinalIgnoreCase));
            if (matched is null)
                e.Color = ColorOptions[0].Color;
        }

        base.SetSelectedItem(e, isUpdate);
        ApplySelection(e?.Color);
    }
}
