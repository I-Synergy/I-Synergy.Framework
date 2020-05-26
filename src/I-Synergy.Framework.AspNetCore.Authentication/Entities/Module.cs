using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    public class Module : ClassBase
    {
        public Module()
        {
            AccountModules = new HashSet<AccountModule>();
        }

        [Required] [Identity] public Guid ModuleId { get; set; }
        [Required] [StringLength(32)] public string Name { get; set; }
        [Required] [StringLength(128)] public string Description { get; set; }
        [Required] public bool IsActive { get; set; }

        public ICollection<AccountModule> AccountModules { get; set; }
    }
}
