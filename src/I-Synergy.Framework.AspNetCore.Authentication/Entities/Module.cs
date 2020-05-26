using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    /// <summary>
    /// Class Module.
    /// Implements the <see cref="ClassBase" />
    /// </summary>
    /// <seealso cref="ClassBase" />
    public class Module : ClassBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module()
        {
            AccountModules = new HashSet<AccountModule>();
        }

        /// <summary>
        /// Gets or sets the module identifier.
        /// </summary>
        /// <value>The module identifier.</value>
        [Required] [Identity] public Guid ModuleId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required] [StringLength(32)] public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required] [StringLength(128)] public string Description { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Required] public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the account modules.
        /// </summary>
        /// <value>The account modules.</value>
        public ICollection<AccountModule> AccountModules { get; set; }
    }
}
