using ISynergy.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Controls.Views
{
    public abstract class View : Page, IView
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as IViewModel)?.OnActivate(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            (DataContext as IViewModel)?.OnDeactivate();
        }
    }
}
