using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Models.Base;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
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

        public ViewModelDialog(
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

            Messenger.Default.Register<OnCancelMessage>(this, (e) =>
            {
                IsCancelled = true;
                CloseWindow();
            });
        }

        protected virtual void CloseWindow()
        {
            Messenger.Default.Send(new CloseWindowsMessage(this));
        }

        public override Task<bool> ValidateInputAsync()
        {
            if (SelectedItem is IModelBase item)
            {
                ValidationService.ValidateProperties(item.GetType());
            }

            return base.ValidateInputAsync();
        }

        public abstract Task SubmitAsync(TEntity e);
    }
}