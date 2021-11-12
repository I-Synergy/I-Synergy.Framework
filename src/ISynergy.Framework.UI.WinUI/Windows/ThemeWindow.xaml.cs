using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Abstractions.Windows;
using ISynergy.Framework.UI.ViewModels;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class ThemeWindow. This class cannot be inherited.
    /// </summary>
    public sealed partial class ThemeWindow : ISynergy.Framework.UI.Controls.Window, IThemeWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeWindow"/> class.
        /// </summary>
        public ThemeWindow()
        {
            InitializeComponent();

            DataContextChanged += ThemeWindow_DataContextChanged;

            PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Save");
            SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Close");
        }

#if WINDOWS_UWP || WINDOWS
        private void ThemeWindow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            SetColorButton();
        }
#else
        private void ThemeWindow_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            SetColorButton();
        }
#endif

        private void SetColorButton()
        {
            if (DataContext is ThemeViewModel themeViewModel)
            {
                switch (themeViewModel.SelectedItem)
                {
                    case Mvvm.Enumerations.ThemeColors.RoyalBlue:
                        Button_Color_RoyalBlue.IsChecked = true;
                        break;
                    case Mvvm.Enumerations.ThemeColors.Lime:
                        Button_Color_Lime.IsChecked = true;
                        break;
                    case Mvvm.Enumerations.ThemeColors.Maroon:
                        Button_Color_Maroon.IsChecked = true;
                        break;
                    case Mvvm.Enumerations.ThemeColors.OrangeRed:
                        Button_Color_OrangeRed.IsChecked = true;
                        break;
                    case Mvvm.Enumerations.ThemeColors.Gold:
                        Button_Color_Gold.IsChecked = true;
                        break;
                    case Mvvm.Enumerations.ThemeColors.Magenta:
                        Button_Color_Magenta.IsChecked = true;
                        break;
                    default:
                        Button_Color_Default.IsChecked = true;
                        break;
                }
            }
        }
    }
}
