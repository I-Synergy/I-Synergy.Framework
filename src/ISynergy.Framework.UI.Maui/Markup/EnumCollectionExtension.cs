using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.UI.Markup;

[Bindable(BindableSupport.Yes)]
[ContentProperty(nameof(EnumType))]
public class EnumCollectionExtension : IMarkupExtension<List<Enum>>
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public required Type EnumType { get; set; }

    public EnumCollectionExtension()
        : base()
    {
    }

    public EnumCollectionExtension([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type enumType)
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
