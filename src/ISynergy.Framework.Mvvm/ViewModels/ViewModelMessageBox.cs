using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm
{
    /// <summary>
    /// Class ViewModelMessageBox.
    /// Implements the <see cref="ViewModelDialog{MessageBoxResult}" />
    /// </summary>
    /// <seealso cref="ViewModelDialog{MessageBoxResult}" />
    public abstract class ViewModelMessageBox : ViewModelDialog<MessageBoxResult>
    {
        /// <summary>
        /// Gets or sets the Message property value.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ButtonOptions property value.
        /// </summary>
        /// <value>The button options.</value>
        public MessageBoxButton ButtonOptions
        {
            get { return GetValue<MessageBoxButton>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ButtonImages property value.
        /// </summary>
        /// <value>The button images.</value>
        public MessageBoxImage ButtonImages
        {
            get { return GetValue<MessageBoxImage>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the OkVisible property value.
        /// </summary>
        /// <value><c>true</c> if [ok visible]; otherwise, <c>false</c>.</value>
        public bool OkVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CancelVisible property value.
        /// </summary>
        /// <value><c>true</c> if [cancel visible]; otherwise, <c>false</c>.</value>
        public bool CancelVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the YesNoVisible property value.
        /// </summary>
        /// <value><c>true</c> if [yes no visible]; otherwise, <c>false</c>.</value>
        public bool YesNoVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsOkDefault property value.
        /// </summary>
        /// <value><c>true</c> if this instance is ok default; otherwise, <c>false</c>.</value>
        public bool IsOkDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsYesDefault property value.
        /// </summary>
        /// <value><c>true</c> if this instance is yes default; otherwise, <c>false</c>.</value>
        public bool IsYesDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsNoDefault property value.
        /// </summary>
        /// <value><c>true</c> if this instance is no default; otherwise, <c>false</c>.</value>
        public bool IsNoDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsCancelDefault property value.
        /// </summary>
        /// <value><c>true</c> if this instance is cancel default; otherwise, <c>false</c>.</value>
        public bool IsCancelDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ok command.
        /// </summary>
        /// <value>The ok command.</value>
        public Command Ok_Command { get; set; }
        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public Command Cancel_Command { get; set; }
        /// <summary>
        /// Gets or sets the yes command.
        /// </summary>
        /// <value>The yes command.</value>
        public Command Yes_Command { get; set; }
        /// <summary>
        /// Gets or sets the no command.
        /// </summary>
        /// <value>The no command.</value>
        public Command No_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelMessageBox"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        protected ViewModelMessageBox(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
            : base(context, commonServices, loggerFactory)
        {
            Title = title;
            Message = message;
            ButtonOptions = button;
            ButtonImages = image;

            Ok_Command = new Command(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.OK)));
            Cancel_Command = new Command(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.Cancel)));
            Yes_Command = new Command(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.Yes)));
            No_Command = new Command(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.No)));
        }

        /// <summary>
        /// Sets the button default.
        /// </summary>
        /// <param name="defaultResult">The default result.</param>
        protected void SetButtonDefault(MessageBoxResult defaultResult)
        {
            switch (defaultResult)
            {
                case MessageBoxResult.OK:
                    IsOkDefault = true;
                    break;

                case MessageBoxResult.Yes:
                    IsYesDefault = true;
                    break;

                case MessageBoxResult.No:
                    IsNoDefault = true;
                    break;

                case MessageBoxResult.Cancel:
                    IsCancelDefault = true;
                    break;
            }
        }

        /// <summary>
        /// Sets the button visibility.
        /// </summary>
        /// <param name="buttonOption">The button option.</param>
        protected void SetButtonVisibility(MessageBoxButton buttonOption)
        {
            switch (buttonOption)
            {
                case MessageBoxButton.YesNo:
                    OkVisible = false;
                    CancelVisible = false;
                    YesNoVisible = true;
                    break;

                case MessageBoxButton.YesNoCancel:
                    OkVisible = false;
                    CancelVisible = true;
                    YesNoVisible = true;

                    break;

                case MessageBoxButton.OK:
                    OkVisible = true;
                    CancelVisible = false;
                    YesNoVisible = false;

                    break;

                case MessageBoxButton.OKCancel:
                    OkVisible = true;
                    CancelVisible = true;
                    YesNoVisible = false;

                    break;

                default:
                    OkVisible = false;
                    CancelVisible = false;
                    YesNoVisible = false;

                    break;
            }
        }
    }
}
