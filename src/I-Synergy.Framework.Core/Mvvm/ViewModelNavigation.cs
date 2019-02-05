using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Models.Base;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract class ViewModelNavigation<TEntity> : ViewModel, IViewModelNavigation<TEntity>
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

            Messenger.Default.Register<OnSubmitMessage>(this, async (e) => await OnSubmitAsync(e));
            Messenger.Default.Register<OnCancelMessage>(this, async (e) => await OnCancelAsync(e));
        }

        public abstract Task OnSubmitAsync(OnSubmitMessage e);
        public abstract Task OnCancelAsync(OnCancelMessage e);
        public abstract Task SubmitAsync(TEntity e);

        //public override Task OnSubmitAsync(OnSubmittanceMessage e)
        //{
        //    if (!e.Handled)
        //    {
        //        if (NavigationService.CanGoBack)
        //            NavigationService.GoBack();

        //        e.Handled = true;
        //    }

        //    return Task.CompletedTask;
        //}

        //public override Task OnCancelAsync(OnCancellationMessage e)
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
