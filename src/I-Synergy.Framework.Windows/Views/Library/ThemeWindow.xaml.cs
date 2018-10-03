using CommonServiceLocator;
using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Library
{
    public sealed partial class ThemeWindow
    {
        private ThemeViewModel ViewModel => DataContext as ThemeViewModel;

        public ThemeWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
