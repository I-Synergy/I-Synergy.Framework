using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract class BaseNavigationViewModel<TEntity> : ViewModel
    {
        public BaseNavigationViewModel(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            Messenger.Default.Register<OnSubmittanceMessage>(this, async (e) => await OnSubmittanceAsync(e));
            Messenger.Default.Register<OnCancellationMessage>(this, async (e) => await OnCancellationAsync(e));
        }

        public abstract Task OnSubmittanceAsync(OnSubmittanceMessage e);
        public abstract Task OnCancellationAsync(OnCancellationMessage e);
    }
}
