using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Syncfusion.UI.Xaml.NavigationDrawer;

namespace Sample.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : View, IShellView
    {
        /// <summary>
        /// Default constructor to initialize the view
        /// </summary>
        public ShellView(IShellViewModel viewModel, INavigationService navigationService)
            :base(viewModel)
        {
            InitializeComponent();
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
