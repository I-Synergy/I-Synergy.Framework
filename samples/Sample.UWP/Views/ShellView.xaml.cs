using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sample.ViewModels;


namespace Sample.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellView : View, IShellView
{
    private readonly ICommonServices _commonServices;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private bool _isLoaded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    public ShellView(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService)
    {
        InitializeComponent();

        _commonServices = commonServices;
        _dialogService = dialogService;
        _navigationService = navigationService;

        Loaded += OnViewLoaded;
        Unloaded += OnViewUnloaded;
    }

    private async void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            await Task.Delay(100);

            if (DataContext is ShellViewModel viewModel)
            {
                await viewModel.ShellLoadedAsync();
                await viewModel.InitializeFirstRunAsync();
            }
        }
    }

    private void OnViewUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnViewLoaded;
        Unloaded -= OnViewUnloaded;
        _isLoaded = false;
    }

    private async void RootNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        await _navigationService.GoBackAsync();
}
