using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
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
                return SynergyService.Language.GetString("Generic_ScanTag");
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
            set { SetValue(value); }
        }


        public TagViewModel(
            IContext context,
            ISynergyService synergyService,
            bool loginVisible = true)
            : base(context, synergyService)
        {
            IsLoginVisible = loginVisible;

            Login_Command = new RelayCommand(() =>
            {
                Messenger.Default.Send(new LoginMessage());
                Messenger.Default.Send(new OnCancellationMessage(this));
            });
        }

        public override void TrackView() { }

        /// <summary>
        /// Gets or sets the Tag property value.
        /// </summary>
        public string RfidTag
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsValid property value.
        /// </summary>
        public bool IsValid
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
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

            Messenger.Default.Send(new OnSubmittanceMessage(this, null));

            return Task.CompletedTask;
        }
    }
}
