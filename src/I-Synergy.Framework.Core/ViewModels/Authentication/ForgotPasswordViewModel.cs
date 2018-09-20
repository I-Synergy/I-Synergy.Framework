using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Common.Handlers;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public class ForgotPasswordViewModel : ViewModelDialog<object>
    {
        public override string Title { get { return ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Password_Forgot"); } }

        /// <summary>
        /// Gets or sets the EmailAddress property value.
        /// </summary>
        public string EmailAddress
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);

                if (value != null && value != "" && NetworkHandler.IsValidEMail(value))
                {
                    Mail_Valid = true;
                }
                else
                {
                    Mail_Valid = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mail_Valid property value.
        /// </summary>
        public bool Mail_Valid
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public ForgotPasswordViewModel(IContext context, IBusyService busy)
            : base(context, busy)
        {
            EmailAddress = "";
        }

        private async Task<bool> CheckFields()
        {
            bool result = true;

            if (EmailAddress is null || !NetworkHandler.IsValidEMail(EmailAddress))
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Invalid_Email"));
                result = false;
            }

            return result;
        }

        public override async Task SubmitAsync(object e)
        {
            if (await CheckFields())
            {
                await Busy.StartBusyAsync();

                var result = await ServiceLocator.Current.GetInstance<IBaseRestService>()?.ForgotPasswordExternal(EmailAddress);

                await Busy.EndBusyAsync();

                Messenger.Default.Send(new OnSubmittanceMessage(this, null));
            }
        }
    }
}