using GalaSoft.MvvmLight.Command;
using ISynergy.Models.Base;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract class ViewModelNavigation<TEntity> : BaseNavigationViewModel<TEntity>, IViewModelNavigation<TEntity>
        where TEntity : class, new()
    {
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

        public ViewModelNavigation(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            SelectedItem = new TEntity();
            IsNew = true;

            Submit_Command = new RelayCommand<TEntity>(async (e) => await SubmitAsync(e));
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
                    await BaseService.Dialog.ShowErrorAsync(BaseService.Language.GetString("Warning_Validation_Failed"));
                    return false;
                }
            }

            return true;
        }

        public abstract Task SubmitAsync(TEntity e);

        //public override Task OnSubmittanceAsync(OnSubmittanceMessage e)
        //{
        //    if (!e.Handled)
        //    {
        //        if (NavigationService.CanGoBack)
        //            NavigationService.GoBack();

        //        e.Handled = true;
        //    }

        //    return Task.CompletedTask;
        //}

        //public override Task OnCancellationAsync(OnCancellationMessage e)
        //{
        //    if (!e.Handled)
        //    {
        //        IsCancelled = true;

        //        if (NavigationService.CanGoBack)
        //            NavigationService.GoBack();

        //        e.Handled = true;
        //    }

        //    return Task.CompletedTask;
        //}
    }
}
