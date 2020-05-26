using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Windows.ViewModels
{
    public class TagViewModel : ViewModelDialog<bool>
    {
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("ScanTag");
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
        /// Gets or sets the Property value.
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

        public TagViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            AuthenticateUserMessageRequest request)
            : base(context, commonServices, loggerFactory)
        {
            IsLoginVisible = request.EnableLogin;
            Property = request.Property;

            Login_Command = new RelayCommand(() =>
            {
                Messenger.Default.Send(new AuthenticateUserMessageRequest(this, true));
            });
        }

        public override Task SubmitAsync(bool e)
        {
            if (Regex.IsMatch(RfidTag, GenericConstants.RfidUidRegeEx))
            {
                e = true;
            }
            else
            {
                e = false;
                RfidTag = string.Empty;
            }

            return base.SubmitAsync(e);
        }
    }
}
