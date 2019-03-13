using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.Mvvm
{
    public abstract class ViewModelDialog<TEntity> : ViewModel, IViewModelDialog<TEntity>
        where TEntity : class, new()
    {
        public IDialogService DialogService { get; }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        public TEntity SelectedItem
        {
            get { return GetValue<TEntity>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsNew property value.
        /// </summary>
        public bool IsNew
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public RelayCommand<TEntity> Submit_Command { get; set; }

        protected ViewModelDialog(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            SelectedItem = new TEntity();
            IsNew = true;

            Submit_Command = new RelayCommand<TEntity>(async (e) =>
            {
                await SubmitAsync(e);

                CloseWindow();
            });

            Messenger.Default.Register<OnCancelMessage>(this, (_) =>
            {
                IsCancelled = true;
                CloseWindow();
            });
        }

        protected virtual void CloseWindow()
        {
            Messenger.Default.Send(new CloseWindowsMessage(this));
        }

        public abstract Task SubmitAsync(TEntity e);
    }
}