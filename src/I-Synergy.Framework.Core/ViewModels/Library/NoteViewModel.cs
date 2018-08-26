using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class NoteViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Note");
            }
        }

        public NoteViewModel(IContext context, IBusyService busy, string note)
            : base(context, busy)
        {
            SelectedItem = note;
        }

        public override Task SubmitAsync(object e)
        {
            Argument.IsNotNull(nameof(e), e);
            Messenger.Default.Send(new OnSubmittanceMessage(this, null));

            return Task.CompletedTask;
        }
    }
}