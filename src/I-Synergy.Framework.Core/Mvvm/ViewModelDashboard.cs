using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Mvvm
{
    public abstract class ViewModelDashboard : ViewModelBladeView<object>, IViewModelBladeView<object>
    {
        public RelayCommand<string> Tile_Command { get; set; }

        protected ViewModelDashboard(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            Tile_Command = new RelayCommand<string>((e) => ExecuteTileCommand(e), CanExecuteTileCommand());

            Messenger.Default.Register<RefreshDashboarMessage>(this, async (_) => await RefreshDashboardAsync());
        }

        private void ExecuteTileCommand(string e)
        {
            if(e.StartsWith("Tile_")) e = e.Replace("Tile_", "");
            Messenger.Default.Send(new TileSelectedMessage(this, e));
        }

        private bool CanExecuteTileCommand()
        {
            if (Context.CurrentProfile != null && Context.CurrentProfile.Principal != null && Context.CurrentProfile.Principal.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

        public abstract Task RefreshDashboardAsync();

        public override Task OnSubmitAsync(OnSubmitMessage e)
        {
            if (!e.Handled && e.Sender != null)
            {
                if (!e.Sender.GetType().GetInterfaces().Contains(typeof(IViewModelBlade)) &&
                    e.Sender.GetType().BaseType.Equals(this))
                {
                    if (BaseService.NavigationService.CanGoBack)
                        BaseService.NavigationService.GoBack();

                    e.Handled = true;
                }
            }

            return Task.CompletedTask;
        }

        public override Task OnCancelAsync(OnCancelMessage e)
        {
            if (!e.Handled && e.Sender != null)
            {
                if (!e.Sender.GetType().GetInterfaces().Contains(typeof(IViewModelBlade)) &&
                    e.Sender.GetType().BaseType.Equals(this))
                {
                    IsCancelled = true;

                    if (BaseService.NavigationService.CanGoBack)
                        BaseService.NavigationService.GoBack();

                    e.Handled = true;
                }
            }

            return Task.CompletedTask;
        }
    }
}
