using Sample.ViewModels;

namespace Sample.Components.Layout;
public partial class NavMenu
{
    public MainViewModel? ViewModel { get; private set; }

    public NavMenu(MainViewModel mainViewModel)
    {
        ViewModel = mainViewModel;
    }
}