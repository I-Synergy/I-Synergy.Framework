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
            InitializeComponent();

            PrimaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Ok");
            SecondaryButtonText = SimpleIoc.Default.GetInstance<ILanguageService>().GetString("Generic_Close");
        }
    }
}
