using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Sample.Models;
using Sample.ViewModels;
using System.Globalization;

namespace Sample.Views;

/// <summary>
/// The Settings blade page. It is hosted by the in-page <see cref="ISynergy.Framework.UI.Controls.BladePane"/>
/// (overlay on tablet/desktop, modal on phone) so the page underneath stays visible behind the scrim.
/// Language, theme and accent color are applied live and persisted as they change.
/// </summary>
[Lifetime(Lifetimes.Scoped)]
public partial class SettingsView
{
    private ISettingsService? _settingsService;
    private IThemeService? _themeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsView"/> class.
    /// </summary>
    public SettingsView()
    {
        InitializeComponent();
    }

    private void EnsureServices()
    {
        if (_settingsService is not null && _themeService is not null)
            return;

        var services = IPlatformApplication.Current?.Services;

        if (services is null)
            return;

        _settingsService ??= services.GetService<ISettingsService>();
        _themeService ??= services.GetService<IThemeService>();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        EnsureServices();

        if (BindingContext is not SettingsViewModel viewModel || viewModel.LocalSettings is not LocalSettings local)
            return;

        // The picker raises SelectionChanged while it is being re-bound — for example when the blade
        // pane re-parents this page's content — and the language is still empty at that moment.
        if (string.IsNullOrWhiteSpace(local.Language))
            return;

        _settingsService?.SaveLocalSettings();

        var culture = new CultureInfo(local.Language);
        culture.ApplyCulture();

        // Update FlowDirection for RTL languages (Arabic).
        if (Application.Current?.Windows.Count > 0)
        {
            var window = Application.Current.Windows[0];
            window.FlowDirection = local.Language == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        EnsureServices();

        if (BindingContext is not SettingsViewModel viewModel || viewModel.LocalSettings is not LocalSettings local)
            return;

        // Apply the theme live via IThemeService (handles persistence and updating UI resources).
        _themeService?.SetTheme(local.Theme);
    }

    private void OnColorClicked(object? sender, EventArgs e)
    {
        EnsureServices();

        if (sender is not Button { CommandParameter: string color })
            return;

        if (BindingContext is not SettingsViewModel viewModel || viewModel.LocalSettings is not LocalSettings local)
            return;

        local.Color = color;

        // Apply the accent color live via IThemeService (handles persistence and updating UI resources).
        _themeService?.SetAccentColor(color);

        _settingsService?.SaveLocalSettings();
        UpdateColorSelection(color);
    }

    private void UpdateColorSelection(string selectedColor)
    {
        var colors = new[]
        {
            (ColorTeal, "#0B5D55"),
            (ColorBlue, "#0066CC"),
            (ColorGreen, "#10B981"),
            (ColorOrange, "#FF8C00")
        };

        foreach (var (button, color) in colors)
        {
            var isSelected = selectedColor == color;
            button.BorderColor = isSelected ? Colors.White : Colors.Gray;
            button.BorderWidth = isSelected ? 3 : 0;
        }
    }
}
