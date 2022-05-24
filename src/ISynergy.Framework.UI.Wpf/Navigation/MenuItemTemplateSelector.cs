using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Navigation
{
    /// <summary>
    /// Class MenuItemTemplateSelector.
    /// </summary>
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the item template.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate { get; set; }

        /// <summary>
        /// Selects the template.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is NavigationItem)
            {
                return ItemTemplate;
            }

            return null;
        }
    }
}
