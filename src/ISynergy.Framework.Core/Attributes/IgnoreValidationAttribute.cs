namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// When this attribute is used, validation of the property is skipped.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class IgnoreValidationAttribute : Attribute
{
}
