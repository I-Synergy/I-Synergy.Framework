using ISynergy.Framework.UI.Models;

namespace ISynergy.Framework.UI.Abstractions.Services;
public interface INavigationMenuService
{
    public IReadOnlyList<NavigationItem> MenuItems { get; init; }

    public IReadOnlyList<NavigationItem> FlattenedMenuItems { get; init; }
}
