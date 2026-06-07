using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Registers handler mappings that override the WinUI focus border color on
/// text input controls (Entry, Editor, Picker, SearchBar, DatePicker, TimePicker)
/// to use the MAUI <c>Application.AccentColor</c> instead of the OS system accent.
/// Only the bottom border is styled (2px accent); top/left/right thickness is zeroed.
/// </summary>
public static class ControlAccentStyler
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
            return;

        _registered = true;

        // Entry → MauiPasswordTextBox → TextBox
        EntryHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyTextBoxAccent(handler.PlatformView));

        // Editor → TextBox
        EditorHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyTextBoxAccent(handler.PlatformView));

        // Picker → ComboBox
        PickerHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyComboBoxAccent(handler.PlatformView));

        // SearchBar → AutoSuggestBox (inner TextBox via Loaded)
        SearchBarHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyAutoSuggestBoxAccent(handler.PlatformView));

        // DatePicker → CalendarDatePicker
        DatePickerHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyCalendarDatePickerAccent(handler.PlatformView));

        // TimePicker → TimePicker
        TimePickerHandler.Mapper.AppendToMapping(
            "AccentFocusBorder",
            (handler, view) => ApplyTimePickerAccent(handler.PlatformView));
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

        comboBox.Resources["ComboBoxBorderBrushFocused"] = ToBrush(accent);
        comboBox.Resources["ComboBoxBorderThemeThicknessFocused"] =
            new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
    }

    private static void ApplyAutoSuggestBoxAccent(object? platformView)
    {
        if (platformView is not AutoSuggestBox suggestBox)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        suggestBox.Loaded += (_, _) =>
        {
            if (suggestBox.FindName("TextBox") is TextBox innerTextBox)
            {
                innerTextBox.Resources["TextControlBorderBrushFocused"] = ToBrush(accent);
                innerTextBox.Resources["TextControlBorderThemeThicknessFocused"] =
                    new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
            }
        };
    }

    private static void ApplyCalendarDatePickerAccent(object? platformView)
    {
        if (platformView is not Microsoft.UI.Xaml.Controls.CalendarDatePicker picker)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        picker.Resources["TextControlBorderBrushFocused"] = ToBrush(accent);
        picker.Resources["TextControlBorderThemeThicknessFocused"] =
            new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
    }

    private static void ApplyTimePickerAccent(object? platformView)
    {
        if (platformView is not Microsoft.UI.Xaml.Controls.TimePicker picker)
            return;

        if (Microsoft.Maui.Controls.Application.AccentColor is not { } accent)
            return;

        picker.Resources["TextControlBorderBrushFocused"] = ToBrush(accent);
        picker.Resources["TextControlBorderThemeThicknessFocused"] =
            new Microsoft.UI.Xaml.Thickness(0, 0, 0, 2);
    }

    private static Microsoft.UI.Xaml.Media.SolidColorBrush ToBrush(Microsoft.Maui.Graphics.Color c) =>
        new(global::Windows.UI.Color.FromArgb(
            (byte)(c.Alpha * 255),
            (byte)(c.Red * 255),
            (byte)(c.Green * 255),
            (byte)(c.Blue * 255)));
}
