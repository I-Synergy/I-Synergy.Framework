using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Globalization;

namespace Sample.Views.Controls;

/// <summary>
/// The Settings drawer. It is part of the current page (last child of the page's root grid),
/// so it slides in over the page — the page stays visible behind the scrim — instead of
/// opening in a separate modal window. Language, theme and accent color are applied live and
/// persisted when the drawer closes.
/// </summary>
public partial class SettingsDrawer : ContentView
{
	private SettingsDrawerState? _state;
	private SettingsViewModel? _settings;
	private ISettingsService? _settingsService;
	private IThemeService? _themeService;
	private IServiceScope? _scope;
	private bool _opening;

	/// <summary>
	/// Initializes a new instance of the <see cref="SettingsDrawer"/> class.
	/// </summary>
	public SettingsDrawer()
	{
		InitializeComponent();

		// The page's binding context is the page view model; the drawer binds to its own
		// settings view model, resolved lazily on the first open so most pages never create one.
		Loaded += OnLoaded;

	}

	private void OnLoaded(object? sender, EventArgs e) => EnsureState();

	private void EnsureState()
	{
		if (_state is not null)
			return;

		var services = IPlatformApplication.Current?.Services;
		if (services is null)
			return;

		_state = services.GetRequiredService<SettingsDrawerState>();
		_state.PropertyChanged += OnStateChanged;

		_themeService = services.GetRequiredService<IThemeService>();

		IsVisible = _state.IsOpen;
		ApplyDeviceWidth();

		if (_state.IsOpen)
			_ = OpenAsync();
	}

	private void OnStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName != nameof(SettingsDrawerState.IsOpen) || _state is null)
			return;

		IsVisible = _state.IsOpen;

		if (_state.IsOpen)
			_ = OpenAsync();
	}

	private async Task OpenAsync()
	{
		if (_opening)
			return;

		_opening = true;
		try
		{
			_settings ??= CreateSettingsViewModel();
			if (_settings is null)
				return;

			BindingContext = _settings;
			await _settings.InitializeAsync();

			UpdateColorSelection(_settings.LocalSettings?.Color ?? string.Empty);
			UpdateDrawerLabels();
		}
		catch
		{
			// Settings are best-effort: an unavailable store leaves the drawer empty rather
			// than failing the page.
		}
		finally
		{
			_opening = false;
		}
	}

	private SettingsViewModel? CreateSettingsViewModel()
	{
		var services = IPlatformApplication.Current?.Services;
		if (services is null)
			return null;

		// SettingsViewModel is scoped, so it is resolved from a scope of its own rather than
		// from the root provider.
		_scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
		_settingsService = _scope.ServiceProvider.GetRequiredService<ISettingsService>();
		return _scope.ServiceProvider.GetRequiredService<SettingsViewModel>();
	}

	private void ApplyDeviceWidth()
	{
		if (DeviceInfo.Idiom != DeviceIdiom.Phone)
			return;

		// Phones have no room beside the drawer: it becomes the page.
		DrawerPanel.WidthRequest = -1;
		DrawerPanel.HorizontalOptions = LayoutOptions.Fill;
	}

	private void OnLanguageChanged(object? sender, EventArgs e)
	{
		if (_settings?.LocalSettings is not LocalSettings local)
			return;

		_settingsService?.SaveLocalSettings();
		var culture = new System.Globalization.CultureInfo(local.Language);
		culture.ApplyCulture();

		// ApplyCulture() calls LocalizedStrings.Default.Refresh() automatically,
		// so UI bindings are updated. Update drawer labels immediately after refresh.
		UpdateDrawerLabels();

		// Update FlowDirection for RTL languages (Arabic)
		if (Application.Current?.Windows.Count > 0)
		{
			var window = Application.Current.Windows[0];
			window.FlowDirection = local.Language == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
		}
	}

	private void OnThemeChanged(object? sender, EventArgs e)
	{
		if (_settings?.LocalSettings is not LocalSettings local)
			return;

		// Apply theme live via IThemeService (handles persistence and updating UI resources)
		if (_themeService is not null)
		{
			_themeService.SetTheme(local.Theme);
		}
	}

	private void OnColorClicked(object? sender, EventArgs e)
	{
		if (sender is not Button { CommandParameter: string color })
			return;

		if (_settings?.LocalSettings is not LocalSettings local)
			return;

		local.Color = color;

		// Apply accent color live via IThemeService (handles persistence and updating UI resources)
		if (_themeService is not null)
		{
			_themeService.SetAccentColor(color);
		}

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

	private void UpdateDrawerLabels()
	{
		var localizedStrings = LocalizedStrings.Default;

		if (this.FindByName<Label>("SettingsHeader") is Label header)
			header.Text = localizedStrings["Settings"];

		if (this.FindByName<Label>("LanguageLabel") is Label languageLabel)
			languageLabel.Text = localizedStrings["Language"];

		if (this.FindByName<Label>("ThemeLabel") is Label themeLabel)
			themeLabel.Text = localizedStrings["Theme"];

		if (this.FindByName<Label>("AccentColorLabel") is Label accentLabel)
			accentLabel.Text = localizedStrings["AccentColor"];

		if (this.FindByName<Button>("CloseButton") is Button closeBtn)
			closeBtn.Text = localizedStrings["Close"];
	}

	private void OnDismissClicked(object? sender, EventArgs e) => Close();

	private void Close()
	{
		if (_state is null && IPlatformApplication.Current?.Services is { } services)
		{
			_state = services.GetRequiredService<SettingsDrawerState>();
			_state.PropertyChanged += OnStateChanged;
		}

		_state?.Close();
	}

	private void OnCloseClicked(object? sender, EventArgs e)
	{
		Close();
	}
}
