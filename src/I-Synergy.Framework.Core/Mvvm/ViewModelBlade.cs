using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.Mvvm
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

        /// <summary>
        /// Gets or sets the IsDisabled property value.
        /// </summary>
        public bool IsDisabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public RelayCommand<TEntity> Submit_Command { get; private set; }

        protected ViewModelBlade(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            SelectedItem = new TEntity();
            IsNew = true;

            Submit_Command = new RelayCommand<TEntity>(async (e) => await SubmitAsync(e));

            Messenger.Default.Register<OnSubmitMessage>(this, async (e) => await OnSubmitAsync(e));
            Messenger.Default.Register<OnCancelMessage>(this, async (e) => await OnCancelAsync(e));
        }

        public abstract Task SubmitAsync(TEntity e);
        public virtual Task OnSubmitAsync(OnSubmitMessage e) => Task.CompletedTask;
        public virtual Task OnCancelAsync(OnCancelMessage e) => Task.CompletedTask;
    }
}
