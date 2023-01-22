using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Syncfusion.UI.Xaml.NavigationDrawer;

namespace Sample.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IShellView
    {
        /// <summary>
        /// Default constructor to initialize the view
        /// </summary>
        public ShellView()
        {
            InitializeComponent();

            ViewModel = ServiceLocator.Default.GetInstance<IShellViewModel>();

            var navigationService = ServiceLocator.Default.GetInstance<INavigationService>();
            navigationService.Frame = ContentRootFrame;
        }

        private void NavigationView_ItemClicked(object sender, NavigationItemClickedEventArgs e)
        {
            if(e.Item.DataContext is ISynergy.Framework.UI.Models.NavigationItem navigationItem)
            {
                if(navigationItem.Command.CanExecute(navigationItem.CommandParameter))
                    navigationItem.Command.Execute(navigationItem.CommandParameter);
            }
        }
    }
}
