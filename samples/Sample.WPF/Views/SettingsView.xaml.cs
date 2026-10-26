using Sample.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sample.Views;

/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsView
{
    public SettingsView()
    {
        InitializeComponent();

        // The page itself is not navigated (blades host its content), so Loaded never fires;
        // DataContextChanged is the reliable point to sync the accent swatches.
        DataContextChanged += (_, _) => UpdateAccentSelection();
    }

    private void OnAccentClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string color } && DataContext is SettingsViewModel viewModel)
        {
            viewModel.SelectedAccentColor = color;
            UpdateAccentSelection();
        }
    }

    private void UpdateAccentSelection()
    {
        if (DataContext is not SettingsViewModel viewModel)
            return;

        UpdateSwatch(AccentTeal, viewModel.SelectedAccentColor);
        UpdateSwatch(AccentBlue, viewModel.SelectedAccentColor);
        UpdateSwatch(AccentGreen, viewModel.SelectedAccentColor);
        UpdateSwatch(AccentOrange, viewModel.SelectedAccentColor);
    }

    private static void UpdateSwatch(Button button, string? selectedColor)
    {
        var isSelected = string.Equals(button.Tag as string, selectedColor, StringComparison.OrdinalIgnoreCase);
        button.BorderBrush = Brushes.White;
        button.BorderThickness = isSelected ? new Thickness(3) : new Thickness(0);
    }
}
