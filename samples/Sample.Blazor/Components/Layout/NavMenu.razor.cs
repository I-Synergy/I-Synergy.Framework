using Microsoft.AspNetCore.Components;
using Sample.ViewModels;

namespace Sample.Components.Layout;
public partial class NavMenu
{
    [Inject]
    public MainViewModel? ViewModel { get; set; }
}