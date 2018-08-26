using CommonServiceLocator;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class LanguageViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Language");
            }
        }

        /// <summary>
        /// Gets or sets the Language property value.
        /// </summary>
        public string Language
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public RelayCommand<string> SetLanguage_Command { get; set; }

        public LanguageViewModel(IContext context, IBusyService busy)
        : base(context, busy)
        {
            SetLanguage_Command = new RelayCommand<string>(SetLanguage);

            Language = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Culture;
        }

        private void SetLanguage(string language)
        {
            Language = language;
        }

        public override Task SubmitAsync(object e)
        {
            ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Culture = Language;
            Messenger.Default.Send(new OnSubmittanceMessage(this, null));

            return Task.CompletedTask;
        }
    }
}