using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.Windows.Navigation
{
    /// <summary>
    /// Class MenuItemTemplateSelector.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.DataTemplateSelector" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.DataTemplateSelector" />
    [ContentProperty(Name = "ItemTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the item template.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate { get; set; }

        /// <summary>
        /// Selects the template core.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>DataTemplate.</returns>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is NavigationItem)
            {
                return ItemTemplate;
            }

            return null;
        }
    }
}
