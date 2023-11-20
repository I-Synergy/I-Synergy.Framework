using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Microsoft.UI.Xaml.Controls;

namespace Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : View, IShellView
    {
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView" /> class.
        /// </summary>
        public ShellView(INavigationService navigationService)
        {
            InitializeComponent();

            _navigationService = navigationService;
            _navigationService.Frame = ContentRootFrame;
        }

        private async void RootNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => await _navigationService.GoBackAsync();
    }
}
