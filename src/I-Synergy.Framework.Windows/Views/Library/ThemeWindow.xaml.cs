using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using ISynergy.ViewModels.Library;

namespace ISynergy.Views.Library
{
    public sealed partial class ThemeWindow
    {
        private ThemeViewModel ViewModel => DataContext as ThemeViewModel;

        public ThemeWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
