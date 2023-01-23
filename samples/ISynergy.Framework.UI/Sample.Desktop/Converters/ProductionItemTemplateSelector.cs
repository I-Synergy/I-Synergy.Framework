using ISynergy.Framework.Core.Collections;
using Sample.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sample.Converters
{
    public class ProductionItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DocumentTemplate { get; set; }
        public DataTemplate FolderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is TreeNode<Guid, PublicationItem> node)
            {
                switch (node.Data.Type)
                {
                    case Enumerations.PublicationItemTypes.Document:
                        return DocumentTemplate;
                    default:
                        return FolderTemplate;  
                }
            }

            return DocumentTemplate;
        }
    }
}
