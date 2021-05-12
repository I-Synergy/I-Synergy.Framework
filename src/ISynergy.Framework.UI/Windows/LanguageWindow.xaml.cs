using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI
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

#if NETFX_CORE || (NET5_0 && WINDOWS)
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
            if (DataContext is LanguageViewModel languageViewModel)
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
