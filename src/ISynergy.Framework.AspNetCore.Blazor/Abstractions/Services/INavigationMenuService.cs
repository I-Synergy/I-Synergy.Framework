using ISynergy.Framework.AspNetCore.Blazor.Models;

namespace ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;

public interface INavigationMenuService
{
    public IReadOnlyList<NavigationItem> MenuItems { get; init; }

    public IReadOnlyList<NavigationItem> FlattenedMenuItems { get; init; }
}
