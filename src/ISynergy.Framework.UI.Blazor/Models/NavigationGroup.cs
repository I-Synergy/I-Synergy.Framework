namespace ISynergy.Framework.UI.Models;
public record NavigationGroup : NavigationItem
{
    public bool Expanded { get; set; }
    public string Gap { get; init; }
    public IReadOnlyList<NavigationItem> Children { get; }

    public NavigationGroup(object icon, string name, bool expanded, string gap, List<NavigationItem> children)
    {
        Href = null;
        Icon = icon;
        Name = name;
        Expanded = expanded;
        Gap = gap;
        Children = children.AsReadOnly();
    }
}