using System;

namespace ISynergy.Framework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IdentityAttribute : Attribute
    {
        public bool IsIdentity { get; set; }

        public IdentityAttribute()
        {
            IsIdentity = true;
        }
    }
}
