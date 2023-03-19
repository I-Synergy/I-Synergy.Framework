using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;

namespace ISynergy.Framework.UI.Windows;

public partial class LanguageWindow : ILanguageWindow
{
    public LanguageWindow()
    {
        InitializeComponent();
        BindingContextChanged += LanguageWindow_BindingContextChanged;
    }

    private void LanguageWindow_BindingContextChanged(object sender, EventArgs e)
    {
        if (ViewModel is LanguageViewModel languageViewModel && !string.IsNullOrEmpty(languageViewModel.SelectedItem))
        {
            switch (languageViewModel.SelectedItem)
            {
                case "nl":
                    Button_Language_nl.IsChecked = true;
                    break;
                case "de":
                    Button_Language_de.IsChecked = true;
                    break;
                case "fr":
                    Button_Language_fr.IsChecked = true;
                    break;
                default:
                    Button_Language_en.IsChecked = true;
                    break;
            }
        }
    }
}