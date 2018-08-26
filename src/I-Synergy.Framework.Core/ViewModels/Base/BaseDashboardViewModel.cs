using CommonServiceLocator;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract class BaseDashboardViewModel : ViewModelNavigation<object>, IViewModelNavigation<object>
    {
        public RelayCommand<string> Tile_Command { get; set; }

        public BaseDashboardViewModel(IContext context, IBusyService busy)
            : base(context, busy)
        {
            Tile_Command = new RelayCommand<string>((e) => ExecuteTileCommand(e), CanExecuteTileCommand());

            Messenger.Default.Register<RefreshDashboarMessage>(this, async (e) => await RefreshDashboardAsync());
        }

        private void ExecuteTileCommand(string e)
        {
            if(e.StartsWith("Tile_")) e = e.Replace("Tile_", "");
            Messenger.Default.Send(new TileSelectedMessage { TileName = e });
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

        public override Task SubmitAsync(object e)
        {
            return Task.CompletedTask;
        }

        public override Task OnSubmittanceAsync(OnSubmittanceMessage e)
        {
            if (!e.Handled && !e.Sender.GetType().GetInterfaces().Contains(typeof(IViewModelBlade)))
            {
                if (ServiceLocator.Current.GetInstance<INavigationService>().CanGoBack)
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }

        public override Task OnCancellationAsync(OnCancellationMessage e)
        {
            if (!e.Handled && !e.Sender.GetType().GetInterfaces().Contains(typeof(IViewModelBlade)))
            {
                IsCancelled = true;

                if (ServiceLocator.Current.GetInstance<INavigationService>().CanGoBack)
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }
    }
}
