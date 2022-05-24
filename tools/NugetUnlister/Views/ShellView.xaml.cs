using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.NavigationDrawer;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace NugetUnlister.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IShellView
    {
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public IShellViewModel ViewModel => ServiceLocator.Default.GetInstance<IShellViewModel>();

        /// <summary>
        /// Default constructor to initialize the view
        /// </summary>
        public ShellView()
        {
            InitializeComponent();
            DataContext = ViewModel;

            var navigationService = ServiceLocator.Default.GetInstance<INavigationService>();
            navigationService.Frame = ContentRootFrame;
        }

        private void NavigationView_ItemClicked(object sender, NavigationItemClickedEventArgs e)
        {
            if (e.Item.DataContext is ISynergy.Framework.UI.Navigation.NavigationItem navigationItem)
            {
                if (navigationItem.Command.CanExecute(navigationItem.CommandParameter))
                    navigationItem.Command.Execute(navigationItem.CommandParameter);
            }
        }
    }
}
