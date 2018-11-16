using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Library
{
    public sealed partial class LanguageWindow
    {
        private LanguageViewModel ViewModel => DataContext as LanguageViewModel;

        public LanguageWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Close");
        }
    }
}
