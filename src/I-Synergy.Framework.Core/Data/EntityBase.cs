using System;
using System.Reflection;

namespace ISynergy.Framework.Core.Data
{
    /// <summary>
    /// BaseEntity model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class EntityBase : ClassBase, IEntityBase
    {
        public string Memo { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ChangedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ChangedBy { get; set; }

        public EntityBase()
        {
            CreatedBy = string.Empty;
        }
    }

    public static class EntityBaseExtensions
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            try
            {
                return obj.GetType().GetRuntimeProperty(propertyName) != null;
            }
            catch (AmbiguousMatchException)
            {
                // ambiguous means there is more than one result,
                // which means: a method with that name does exist
                return true;
            }
        }
    }
}
