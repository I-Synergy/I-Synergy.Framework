using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Mvvm;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public class LanguageViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return BaseService.LanguageService.GetString("Generic_Language");
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

        public LanguageViewModel(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            SetLanguage_Command = new RelayCommand<string>(SetLanguage);

            Language = BaseService.BaseSettingsService.Application_Culture;
        }

        private void SetLanguage(string language)
        {
            Language = language;
        }

        public override Task SubmitAsync(object e)
        {
            Messenger.Default.Send(new OnLanguageChangedMessage(this, Language));
            return Task.CompletedTask;
        }
    }
}