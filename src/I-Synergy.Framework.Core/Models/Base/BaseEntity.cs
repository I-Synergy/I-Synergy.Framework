using ISynergy.Models.Base;
using System.Reflection;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// BaseEntity model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class BaseEntity : BaseClass, IBaseEntity
    {
        public string Memo { get; set; }
        public string InputFirst { get; set; }
        public string InputLast { get; set; }
    }

    public static class BaseEntity_Extension
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
