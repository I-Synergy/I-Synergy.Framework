using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Overrides the WinUI system accent on native controls so focus borders and
/// selection indicators use MAUI's <c>Application.AccentColor</c>.
/// </summary>
public static class ControlAccentStyler
{
    private static bool _registered;

    private static readonly string[] GlobalAccentKeys =
    [
        // CollectionView selection indicators
        "ListViewItemSelectionIndicatorBrush",
        "ListViewItemSelectionIndicatorPointerOverBrush",
        "ListViewItemSelectionIndicatorPressedBrush",
        "ListViewItemCheckBoxSelectedBrush",
        "ListViewItemCheckBoxSelectedPointerOverBrush",
        "ListViewItemCheckBoxSelectedPressedBrush",
        "GridViewItemSelectedBorderBrush",
        "GridViewItemSelectedPointerOverBorderBrush",
        "GridViewItemSelectedPressedBorderBrush",
        "GridViewItemCheckBoxSelectedBrush",
        "GridViewItemCheckBoxSelectedPointerOverBrush",
        "GridViewItemCheckBoxSelectedPressedBrush",

        // Shell flyout / NavigationView selection indicator
        "NavigationViewSelectionIndicatorForeground",
    ];

    public static void Register()
    {
        if (_registered)
            return;

        _registered = true;

        ApplyToWinUIApplicationResources();

        // Entry / Editor → TextBox
        EntryHandler.Mapper.AppendToMapping(
            "AccentStyler",
            (handler, view) => ApplyTextBoxAccent(handler.PlatformView));
        EditorHandler.Mapper.AppendToMapping(
            "AccentStyler",
            (handler, view) => ApplyTextBoxAccent(handler.PlatformView));

        // Picker → ComboBox
        PickerHandler.Mapper.AppendToMapping(
            "AccentStyler",
            (handler, view) => ApplyComboBoxAccent(handler.PlatformView));

        // SearchBar → AutoSuggestBox
        SearchBarHandler.Mapper.AppendToMapping(
            "AccentStyler",
            (handler, view) => ApplyAutoSuggestBoxAccent(handler.PlatformView));

    }

    private static void ApplyTextBoxAccent(object? platformView)
    {
        if (platformView is not TextBox textBox)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        textBox.Resources["TextControlBorderBrushFocused"] = ToBrush(accent);
        textBox.Resources["TextControlBorderThemeThicknessFocused"] =
            new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
    }

    private static void ApplyComboBoxAccent(object? platformView)
    {
        if (platformView is not ComboBox comboBox)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        var brush = ToBrush(accent);
        comboBox.Resources["ComboBoxBorderBrushFocused"] = brush;
        comboBox.Resources["ComboBoxBorderThemeThicknessFocused"] =
            new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
        comboBox.Resources["ComboBoxItemPillFillBrush"] = brush;
    }

    private static void ApplyAutoSuggestBoxAccent(object? platformView)
    {
        if (platformView is not AutoSuggestBox suggestBox)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        suggestBox.Loaded += (_, _) =>
        {
            var innerTextBox = FindFirstDescendant<TextBox>(suggestBox);
            if (innerTextBox is not null)
            {
                innerTextBox.Resources["TextControlBorderBrushFocused"] = ToBrush(accent);
                innerTextBox.Resources["TextControlBorderThemeThicknessFocused"] =
                    new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
            }
        };
    }

    private static T? FindFirstDescendant<T>(DependencyObject parent) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
                return match;

            var result = FindFirstDescendant<T>(child);
            if (result is not null)
                return result;
        }
        return null;
    }

    private static void ApplyToWinUIApplicationResources()
    {
        if (Microsoft.UI.Xaml.Application.Current is not { } winUIApp)
            return;

        var accent = Microsoft.Maui.Controls.Application.AccentColor;
        if (accent is null)
            return;

        var brush = ToBrush(accent);
        var dict = new Microsoft.UI.Xaml.ResourceDictionary();
        foreach (var key in GlobalAccentKeys)
            dict[key] = brush;

        winUIApp.Resources.MergedDictionaries.Add(dict);
    }

    private static Microsoft.UI.Xaml.Media.SolidColorBrush ToBrush(Microsoft.Maui.Graphics.Color c) =>
        new(global::Windows.UI.Color.FromArgb(
            (byte)(c.Alpha * 255),
            (byte)(c.Red * 255),
            (byte)(c.Green * 255),
            (byte)(c.Blue * 255)));
}
