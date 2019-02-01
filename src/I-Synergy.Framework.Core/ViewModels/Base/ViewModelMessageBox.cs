using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Enumerations;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Library
{
    public abstract class ViewModelMessageBox<TEntity> : ViewModelDialog<TEntity>
        where TEntity : class, new()
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
        /// Gets or sets the Result property value.
        /// </summary>
        public int Result
        {
            get { return GetValue<int>(); }
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

        public ViewModelMessageBox(
            IContext context,
            IBaseService baseService, 
            string message, 
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information,
            MessageBoxResult result = MessageBoxResult.OK)
            : base(context, baseService)
        {
            Title = title;
            Message = message;
            ButtonOptions = button;
        }

        private void SetButtonDefault(MessageBoxResult defaultResult)
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

                default:
                    break;
            }
        }

        private void SetButtonVisibility(MessageBoxButton buttonOption)
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

        public override Task SubmitAsync(TEntity e)
        {
            int.TryParse(e.ToString(), out int parameter);

            Result = parameter;

            Messenger.Default.Send(new OnSubmitMessage(this, e));

            return Task.CompletedTask;
        }
    }
}