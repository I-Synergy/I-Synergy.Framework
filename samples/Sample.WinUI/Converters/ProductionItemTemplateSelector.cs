using ISynergy.Framework.Core.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sample.Models;

namespace Sample.Converters;

public class ProductionItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate DocumentTemplate { get; set; }
    public DataTemplate FolderTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is TreeNode<Guid, PublicationItem> node)
        {
            return node.Data.Type switch
            {
                Enumerations.PublicationItemTypes.Document => DocumentTemplate,
                _ => FolderTemplate,
            };
        }

        return DocumentTemplate;
    }
}
