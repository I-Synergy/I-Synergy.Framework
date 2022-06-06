using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;

#if (WINDOWS_UWP || HAS_UNO)
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
    public sealed partial class LanguageWindow : ISynergy.Framework.UI.Controls.Window, ILanguageWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageWindow"/> class.
        /// </summary>
        public LanguageWindow()
        {
            InitializeComponent();

            DataContextChanged += LanguageWindow_DataContextChanged;

#if !WINDOWS_WPF
            PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Ok");
            SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Close");
#endif
        }

#if WINDOWS_UWP || WINDOWS_WINUI
        private void LanguageWindow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            SetLanguageButton();
        }
#else
        private void LanguageWindow_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            SetLanguageButton();
        }
#endif

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
