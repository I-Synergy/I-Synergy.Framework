using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Markup;

[Bindable(BindableSupport.Yes)]
[ContentProperty(nameof(EnumType))]
public class EnumCollectionExtension : IMarkupExtension<List<Enum>>
{
    public Type EnumType { get; set; }

    public EnumCollectionExtension()
        : base()
    {
    }

    public EnumCollectionExtension(Type enumType)
        : this()
    {
        EnumType = enumType;
    }

    public List<Enum> ProvideValue(IServiceProvider serviceProvider)
    {
        Argument.IsNotNull(EnumType);
        return EnumType.ToList();
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        (this as IMarkupExtension<List<Enum>>).ProvideValue(serviceProvider);
}
