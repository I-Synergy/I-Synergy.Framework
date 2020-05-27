using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.EntityFramework.Entities;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Class BaseNote.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BaseNote : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        [Required] public DateTimeOffset DateTime { get; set; }
    }
}
