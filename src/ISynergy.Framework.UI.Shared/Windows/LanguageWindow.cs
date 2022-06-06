#if WINDOWS_UWP || WINDOWS_WINUI
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#elif WINDOWS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class LanguageWindow. This class cannot be inherited.
    /// </summary>
    public partial class LanguageWindow : ISynergy.Framework.UI.Controls.Window, ILanguageWindow
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
            SetLanguageButton();
        }

        private void SetLanguageButton()
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
}
#endif