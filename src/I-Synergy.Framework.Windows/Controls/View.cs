using ISynergy.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Controls.Views
{
    public abstract class View : Page, IView
    {
        public new IViewModel DataContext
        {
            get { return base.DataContext as IViewModel; }
            set { base.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext?.OnActivate(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            DataContext?.OnDeactivate();
        }
    }
}
