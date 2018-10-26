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

            Submit_Command = new RelayCommand<TEntity>(async (e) => await OkAction(e));

            Messenger.Default.Register<OnCancellationMessage>(this, (e) =>
            {
                IsCancelled = true;
                CloseWindow();
            });
        }

        protected async Task OkAction(TEntity e)
        {
            await SubmitAsync(e);

            if (e is IBaseModel && e != null)
            {
                if (!(e as IBaseModel).HasErrors)
                    Messenger.Default.Send(new OnSubmittanceMessage(this, null));
            }
        }


        //protected override Task CancelAndCloseViewModelAsync()
        //{
        //    SelectedItem = null;
        //    return base.CancelAndCloseViewModelAsync();
        //}

        //protected override Task CloseViewModelAsync()
        //{
        //    CloseWindow();
        //    return Task.CompletedTask;
        //}

        protected virtual void CloseWindow()
        {
            Messenger.Default.Send(new CloseWindowsMessage(this));
        }

        protected virtual async Task<bool> ValidateInputAsync()
        {
            if (SelectedItem is IBaseModel)
            {
                IBaseModel item = SelectedItem as IBaseModel;

                item.ValidateProperties();
                Errors = FlattenErrors();

                if (item.HasErrors)
                {
                    await DialogService.ShowErrorAsync(BaseService.LanguageService.GetString("Warning_Validation_Failed"));
                    return false;
                }
            }

            return true;
        }

        public abstract Task SubmitAsync(TEntity e);
    }
}