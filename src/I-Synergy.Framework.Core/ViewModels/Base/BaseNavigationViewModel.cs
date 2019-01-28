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
            Messenger.Default.Register<OnSubmitMessage>(this, async (e) => await OnSubmitAsync(e));
            Messenger.Default.Register<OnCancelMessage>(this, async (e) => await OnCancelAsync(e));
        }

        public abstract Task OnSubmitAsync(OnSubmitMessage e);
        public abstract Task OnCancelAsync(OnCancelMessage e);
    }
}
