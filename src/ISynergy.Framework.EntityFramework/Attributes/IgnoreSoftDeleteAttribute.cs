namespace ISynergy.Framework.EntityFramework.Attributes;

/// <summary>
/// Indicates that the entity class should not have the soft-delete query filter applied automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IgnoreSoftDeleteAttribute : Attribute
{
}
