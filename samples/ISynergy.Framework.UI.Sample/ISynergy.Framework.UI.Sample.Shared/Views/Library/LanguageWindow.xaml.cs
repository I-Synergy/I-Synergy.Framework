using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Sample.Views.Library
{
    /// <summary>
    /// Class LanguageWindow. This class cannot be inherited.
    /// </summary>
    public sealed partial class LanguageWindow : ILanguageWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageWindow"/> class.
        /// </summary>
        public LanguageWindow()
        {
            InitializeComponent();

            DataContextChanged += LanguageWindow_DataContextChanged;

            PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Ok");
            SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Close");
        }

        private void LanguageWindow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if(DataContext is LanguageViewModel languageViewModel)
            {
                if (!string.IsNullOrEmpty(languageViewModel.SelectedItem))
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
    }
}
