#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endif

namespace ISynergy.Framework.UI.Navigation
{
    /// <summary>
    /// Class MenuItemTemplateSelector.
    /// Implements the <see cref="DataTemplateSelector" />
    /// </summary>
    /// <seealso cref="DataTemplateSelector" />
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
            if (item is NavigationItem)
            {
                return ItemTemplate;
            }

            return null;
        }
    }
}
