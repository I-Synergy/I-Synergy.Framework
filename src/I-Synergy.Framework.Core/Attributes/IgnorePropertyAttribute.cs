using System;

namespace ISynergy.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
    }
}
