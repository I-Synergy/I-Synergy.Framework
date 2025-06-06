using Microsoft.AspNetCore.Components;
using Sample.ViewModels;

namespace Sample.Components.Layout;

public partial class MainLayout
{
    public MainViewModel? ViewModel { get; private set; }
    public NavigationManager NavigationManager { get; private set; }

    public MainLayout(MainViewModel mainViewModel, NavigationManager navigationManager)
    {
        ViewModel = mainViewModel;
        NavigationManager = navigationManager;
    }
}
