using ISynergy.Framework.Core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ISynergy.Framework.UI.Navigation;

/// <summary>
/// Class MenuItemTemplateSelector.
/// Implements the <see cref="DataTemplateSelector" />
/// </summary>
/// <seealso cref="DataTemplateSelector" />
[ContentProperty("ItemTemplate")]
public class MenuItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the item template.
    /// </summary>
    /// <value>The item template.</value>
    public DataTemplate? ItemTemplate { get; set; }

    /// <summary>
    /// Selects the template.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is NavigationItem)
            return ItemTemplate;

        return null;
    }
}
