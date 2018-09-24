using GalaSoft.MvvmLight.Messaging;
using ISynergy.Common.Handlers;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public abstract class BaseForgotPasswordViewModel : ViewModelDialog<object>, IForgotPasswordViewModel
    {
        public override string Title { get { return SynergyService.Language.GetString("Generic_Password_Forgot"); } }

        public BaseForgotPasswordViewModel(
            IContext context,
            ISynergyService synergyService)
            : base(context, synergyService)
        {
            EmailAddress = "";
        }


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

        private async Task<bool> CheckFields()
        {
            bool result = true;

            if (EmailAddress is null || !NetworkHandler.IsValidEMail(EmailAddress))
            {
                await DialogService.ShowErrorAsync(SynergyService.Language.GetString("Warning_Invalid_Email"));
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// public override Task<bool> ResetPasswordAsync()
        /// {
        ///     return RestService?.ForgotPasswordExternal(EmailAddress);
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public abstract Task<bool> ResetPasswordAsync();

        public override async Task SubmitAsync(object e)
        {
            if (await CheckFields())
            {
                await SynergyService.Busy.StartBusyAsync();

                var result = await ResetPasswordAsync();

                await SynergyService.Busy.EndBusyAsync();

                Messenger.Default.Send(new OnSubmittanceMessage(this, null));
            }
        }
    }
}