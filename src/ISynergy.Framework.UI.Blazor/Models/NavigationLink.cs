using Microsoft.AspNetCore.Components.Routing;

namespace ISynergy.Framework.UI.Models;
public record NavigationLink : NavigationItem
{
    public NavigationLink(string? href, object icon, string name, NavLinkMatch match = NavLinkMatch.Prefix)
    {
        Href = href;
        Icon = icon;
        Name = name;
        Match = match;
    }
}
