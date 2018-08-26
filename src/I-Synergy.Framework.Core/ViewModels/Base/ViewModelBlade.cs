using CommonServiceLocator;
using GalaSoft.MvvmLight.Command;
using ISynergy.Models.Base;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract class ViewModelBlade<TEntity> : ViewModel, IViewModelBlade
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
        /// Gets or sets the Owner property value.
        /// </summary>
        public object Owner
        {
            get { return GetValue<object>(); }
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

        public RelayCommand<TEntity> Submit_Command { get; private set; }

        public ViewModelBlade(IContext context, IBusyService busy)
            : base(context, busy)
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
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Validation_Failed"));
                    return false;
                }
            }

            return true;
        }

        public abstract Task SubmitAsync(TEntity e);

        //public override Task OnSubmittanceAsync(OnSubmittanceMessage e) => Task.CompletedTask;
        //public override Task OnCancellationAsync(OnCancellationMessage e) => Task.CompletedTask;
    }
}
