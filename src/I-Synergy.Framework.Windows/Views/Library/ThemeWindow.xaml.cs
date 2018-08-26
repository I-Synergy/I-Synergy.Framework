// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using CommonServiceLocator;
using ISynergy.Services;

namespace ISynergy.Views.Library
{
    public sealed partial class ThemeWindow
    {
        public ThemeWindow()
        {
            this.InitializeComponent();
            this.PrimaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Cancel");
        }
    }
}
