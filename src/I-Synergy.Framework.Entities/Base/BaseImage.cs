using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.EntityFramework.Entities;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Class BaseImage.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BaseImage : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        [Required] public DateTimeOffset DateTime { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required] [StringLength(255)] public string Description { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [Required] public string Url { get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        [Required] [StringLength(128)] public string ContentType { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [StringLength(255)] public string Subject { get; set; }
    }
}
