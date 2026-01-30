using ISynergy.Framework.Core.Attributes;
using DescriptionAttribute = ISynergy.Framework.Core.Attributes.DescriptionAttribute;

namespace ISynergy.Framework.Core.Tests.Data.TestClasses;

/// <summary>
/// Enum ProductTypes
/// </summary>
public enum ProductTypes
{
    /// <summary>
    /// The type1
    /// </summary>
    [Description("Description1")]
    Type1,
    /// <summary>
    /// The type2
    /// </summary>
    [Description("Description2")]
    Type2,
    /// <summary>
    /// The type3
    /// </summary>
    [Description("Description3")]
    Type3
}
