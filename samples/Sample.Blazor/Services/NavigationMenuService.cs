using ISynergy.Framework.UI.Models;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.AspNetCore.Components.Routing;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace Sample.Services;

public class NavigationMenuService : BaseNavigationMenuService
{
    public NavigationMenuService()
    {
        MenuItems =
        [
            new NavigationLink(
                href: "/",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.Home(),
                name: "Home"
            ),
            new NavigationLink(
                href: "/counter",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.TextWordCount(),
                name: "Counter"
            ),
            new NavigationLink(
                href: "/weather",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.WeatherRain(),
                name: "Weather"
            ),
            new NavigationLink(
                href: "/commanddemoview",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.KeyCommand(),
                name: "Command Demo"
            )];
    }
}