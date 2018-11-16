using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using ISynergy.ViewModels.Library;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Views.Library
{
    public sealed partial class NoteWindow : INoteWindow
    {
        private NoteViewModel ViewModel => DataContext as NoteViewModel;

        public NoteWindow()
        {
            this.InitializeComponent();

            this.PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            this.SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Close");
        }
    }
}
