using ISynergy.Framework.Mvvm.Abstractions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Sample.ViewModels;

namespace Sample.Views;

public partial class SettingsView : IView
{
    public SettingsView()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateAccentSelection();
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
        button.BorderBrush = new SolidColorBrush(Colors.White);
        button.BorderThickness = new Thickness(isSelected ? 3 : 0);
    }
}
