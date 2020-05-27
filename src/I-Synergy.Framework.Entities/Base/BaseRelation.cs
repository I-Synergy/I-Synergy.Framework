using ISynergy.Framework.EntityFramework.Entities;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Class BaseRelation.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public class BaseRelation : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        [Required] public int Number { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [Required] [StringLength(10)] public string Code { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is corporate.
        /// </summary>
        /// <value><c>true</c> if this instance is corporate; otherwise, <c>false</c>.</value>
        [Required] public bool IsCorporate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is private.
        /// </summary>
        /// <value><c>true</c> if this instance is private; otherwise, <c>false</c>.</value>
        [Required] public bool IsPrivate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }
    }
}
