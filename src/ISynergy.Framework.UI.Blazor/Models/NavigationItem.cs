using Microsoft.AspNetCore.Components.Routing;

namespace ISynergy.Framework.UI.Models;
public abstract record NavigationItem
{
    public string Name { get; init; } = string.Empty;
    public string? Href { get; init; }
    public NavLinkMatch Match { get; init; } = NavLinkMatch.Prefix;
    public object? Icon { get; init; }
}
