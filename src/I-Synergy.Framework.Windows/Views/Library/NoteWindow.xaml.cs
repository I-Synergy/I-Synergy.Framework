using CommonServiceLocator;
using ISynergy.Services;

namespace ISynergy.Views.Library
{
    public sealed partial class NoteWindow : INoteWindow
    {
        public NoteWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Close");
        }
    }
}
