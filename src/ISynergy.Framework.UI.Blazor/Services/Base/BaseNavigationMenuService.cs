using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Models;

namespace ISynergy.Framework.UI.Services.Base;
public abstract class BaseNavigationMenuService : INavigationMenuService
{
    internal const string EditFormOffIcon = "<svg style=\"width: 12px; fill: var(--neutral-foreground-rest);\" focusable=\"false\" viewBox=\"0 0 16 16\" aria-hidden=\"true\"><title>This component is not yet compatible with the EditForm.</title><!--!--><path d=\"M5.8 6.5 1.14 1.85a.5.5 0 1 1 .7-.7l13 13a.5.5 0 0 1-.7.7L9.5 10.21l-3.14 3.13c-.37.38-.84.64-1.35.78l-3.39.86a.5.5 0 0 1-.6-.6l.86-3.39c.14-.51.4-.98.78-1.35L5.79 6.5Zm3 3L6.5 7.2l-3.14 3.14c-.24.25-.42.56-.5.9l-.67 2.57 2.57-.66c.34-.1.65-.27.9-.51L8.79 9.5Zm3.24-3.25-1.83 1.84.7.7 3.33-3.32a2.62 2.62 0 0 0-3.71-3.7L7.2 5.08l.7.7 1.84-1.83 2.3 2.29Zm-.8-3.78a1.62 1.62 0 1 1 2.29 2.3l-.78.77-2.3-2.29.79-.78Z\"></path></svg>";

    public IReadOnlyList<NavigationItem> MenuItems { get; init; }

    public IReadOnlyList<NavigationItem> FlattenedMenuItems { get; init; }
}
