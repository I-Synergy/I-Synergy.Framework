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
            this.PrimaryButtonText = ActivatorUtilities.CreateInstance<ILanguageService>(ViewModel.BaseService.ServiceProvider).GetString("Generic_Cancel");
        }
    }
}
