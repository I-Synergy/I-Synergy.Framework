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
            InitializeComponent();

            PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Close");
        }
    }
}
