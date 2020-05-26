using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm
{
    public abstract class ViewModelMessageBox : ViewModelDialog<MessageBoxResult>
    {
        /// <summary>
        /// Gets or sets the Message property value.
        /// </summary>
        public string Message
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ButtonOptions property value.
        /// </summary>
        public MessageBoxButton ButtonOptions
        {
            get { return GetValue<MessageBoxButton>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ButtonImages property value.
        /// </summary>
        public MessageBoxImage ButtonImages
        {
            get { return GetValue<MessageBoxImage>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the OkVisible property value.
        /// </summary>
        public bool OkVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CancelVisible property value.
        /// </summary>
        public bool CancelVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the YesNoVisible property value.
        /// </summary>
        public bool YesNoVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsOkDefault property value.
        /// </summary>
        public bool IsOkDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsYesDefault property value.
        /// </summary>
        public bool IsYesDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsNoDefault property value.
        /// </summary>
        public bool IsNoDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsCancelDefault property value.
        /// </summary>
        public bool IsCancelDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public RelayCommand Ok_Command { get; set; }
        public RelayCommand Cancel_Command { get; set; }
        public RelayCommand Yes_Command { get; set; }
        public RelayCommand No_Command { get; set; }

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

            Ok_Command = new RelayCommand(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.OK)));
            Cancel_Command = new RelayCommand(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.Cancel)));
            Yes_Command = new RelayCommand(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.Yes)));
            No_Command = new RelayCommand(() => OnSubmitted(new SubmitEventArgs<MessageBoxResult>(MessageBoxResult.No)));
        }

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
