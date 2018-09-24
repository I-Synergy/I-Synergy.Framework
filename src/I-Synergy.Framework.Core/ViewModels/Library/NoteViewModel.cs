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
                return SynergyService.Language.GetString("Generic_Note");
            }
        }

        public NoteViewModel(
            IContext context,
            ISynergyService synergyService,
            string note)
            : base(context, synergyService)
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