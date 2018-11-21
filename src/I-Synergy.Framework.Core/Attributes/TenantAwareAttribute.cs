using System;

namespace ISynergy.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TenantAwareAttribute : Attribute
    {
        public const string TenantAnnotation = "TenantAnnotation";
        public const string Tenant_IdFilterParameterName = "Tenant_IdParameter";

        public string FieldName { get; set; }

        public TenantAwareAttribute(string field)
        {
            FieldName = field;
        }
    }
}