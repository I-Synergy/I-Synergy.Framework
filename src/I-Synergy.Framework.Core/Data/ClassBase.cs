using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Data
{
    /// <summary>
    /// Base_Class model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class ClassBase : IClassBase
    {
        [Required]
        public int Version { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
