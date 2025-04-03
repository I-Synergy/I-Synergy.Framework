using ISynergy.Framework.Core.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sample.Models;

namespace Sample.Converters;

public class ProductionItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate DocumentTemplate { get; set; } = new();
    public DataTemplate FolderTemplate { get; set; } = new();

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is TreeNode<Guid, PublicationItem> node && node.Data is not null)
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
