using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Mvvm;
using ISynergy.Services;
using System;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class NoteViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return BaseService.LanguageService.GetString("Generic_Note");
            }
        }

        public NoteViewModel(
            IContext context,
            IBaseService baseService,
            string note)
            : base(context, baseService)
        {
            SelectedItem = note;
        }

        public override Task SubmitAsync(object e)
        {
            Argument.IsNotNull(nameof(e), e);
            Messenger.Default.Send(new OnSubmitMessage(this, e));

            return Task.CompletedTask;
        }
    }
}