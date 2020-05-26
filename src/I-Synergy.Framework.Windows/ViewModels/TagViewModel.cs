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
    /// <summary>
    /// Class TagViewModel.
    /// Implements the <see cref="ViewModelDialog{bool}" />
    /// </summary>
    /// <seealso cref="ViewModelDialog{bool}" />
    public class TagViewModel : ViewModelDialog<bool>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
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
        /// <value>The login command.</value>
        public RelayCommand Login_Command
        {
            get { return GetValue<RelayCommand>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsLoginVisible property value.
        /// </summary>
        /// <value><c>true</c> if this instance is login visible; otherwise, <c>false</c>.</value>
        public bool IsLoginVisible
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Property value.
        /// </summary>
        /// <value>The property.</value>
        public object Property
        {
            get { return GetValue<object>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Tag property value.
        /// </summary>
        /// <value>The rfid tag.</value>
        public string RfidTag
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="request">The request.</param>
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

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">if set to <c>true</c> [e].</param>
        /// <returns>Task.</returns>
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
