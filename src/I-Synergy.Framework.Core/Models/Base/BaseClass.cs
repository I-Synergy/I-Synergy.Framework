using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Base_Class model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseClass : IBaseClass
    {
        [Required]
        public bool IsDeleted { get; set; }
    }
}