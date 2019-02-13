using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ISynergy
{
    /// <summary>
    /// Base_Class model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class ClassBase : IClassBase
    {
        [Required]
        public bool IsDeleted { get; set; }
    }
}