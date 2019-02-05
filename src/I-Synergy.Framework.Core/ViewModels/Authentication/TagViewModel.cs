using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Mvvm;
using ISynergy.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public class TagViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get
            {
                return BaseService.LanguageService.GetString("Generic_ScanTag");
            }
        }

        /// <summary>
        /// Gets or sets the Login_Command property value.
        /// </summary>
        public RelayCommand Login_Command
        {
            get { return GetValue<RelayCommand>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsLoginVisible property value.
        /// </summary>
        public bool IsLoginVisible
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Property property value.
        /// </summary>
        public object Property
        {
            get { return GetValue<object>(); }
            private set { SetValue(value); }
        }


        /// <summary>
        /// Gets or sets the Tag property value.
        /// </summary>
        public string RfidTag
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsValid property value.
        /// </summary>
        public bool IsValid
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }

        public TagViewModel(
            IContext context,
            IBaseService baseService,
            AuthenticateUserMessageRequest request)
            : base(context, baseService)
        {
            IsLoginVisible = request.EnableLogin;
            Property = request.Property;

            Login_Command = new RelayCommand(() =>
            {
                Messenger.Default.Send(new AuthenticateUserMessageRequest(this, true));
                Messenger.Default.Send(new OnCancelMessage(this));
            });
        }

        public override Task SubmitAsync(object e)
        {
            if (Regex.IsMatch(RfidTag, Constants.RfidUidRegeEx))
            {
                IsValid = true;
            }
            else
            {
                IsValid = false;
                RfidTag = string.Empty;
            }

            Messenger.Default.Send(new OnSubmitMessage(this, IsValid));

            return Task.CompletedTask;
        }
    }
}
