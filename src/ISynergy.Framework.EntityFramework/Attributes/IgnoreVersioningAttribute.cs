namespace ISynergy.Framework.EntityFramework.Attributes;

/// <summary>
/// Indicates that the entity class should not have row-version concurrency tokens applied automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IgnoreVersioningAttribute : Attribute
{
}
