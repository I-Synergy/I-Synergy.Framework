using ISynergy.ViewModels.Base;
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
