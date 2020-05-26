using System;

namespace ISynergy.Framework.EntityFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TenantAwareAttribute : Attribute
    {
        public const string TenantAnnotation = "TenantAnnotation";
        public const string TenantIdFilterParameterName = "TenantIdParameter";

        public string FieldName { get; }

        public TenantAwareAttribute(string field)
        {
            FieldName = field;
        }
    }
}
