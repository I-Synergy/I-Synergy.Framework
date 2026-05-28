namespace ISynergy.Framework.EntityFramework.Attributes;

/// <summary>
/// Class TenantAwareAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class TenantAwareAttribute : Attribute
{
    /// <summary>
    /// The tenant annotation
    /// </summary>
    public const string TenantAnnotation = "TenantAnnotation";
    /// <summary>
    /// The tenant identifier filter parameter name
    /// </summary>
    public const string TenantIdFilterParameterName = "TenantIdParameter";

    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantAwareAttribute"/> class.
    /// </summary>
    /// <param name="field">The field.</param>
    public TenantAwareAttribute(string field)
    {
        FieldName = field;
    }
}
