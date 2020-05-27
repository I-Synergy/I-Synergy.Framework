using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.EntityFramework.Entities;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Class BasePrice.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BasePrice : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the unit identifier.
        /// </summary>
        /// <value>The unit identifier.</value>
        [Required] public Guid UnitId { get; set; }
        /// <summary>
        /// Gets or sets the unit description.
        /// </summary>
        /// <value>The unit description.</value>
        [Required] [StringLength(50)] public string UnitDescription { get; set; }
        /// <summary>
        /// Gets or sets the unit code.
        /// </summary>
        /// <value>The unit code.</value>
        [Required] [StringLength(6)] public string UnitCode { get; set; }
        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        [Required] public decimal Units { get; set; }
        /// <summary>
        /// Gets or sets the units per packing.
        /// </summary>
        /// <value>The units per packing.</value>
        [Required] public decimal UnitsPerPacking { get; set; }
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        [Required] public decimal Amount { get; set; }
        /// <summary>
        /// Gets or sets the vat code identifier.
        /// </summary>
        /// <value>The vat code identifier.</value>
        [Required] public Guid VATCodeId { get; set; }
        /// <summary>
        /// Gets or sets the vat percentage.
        /// </summary>
        /// <value>The vat percentage.</value>
        [Required] public decimal VATPercentage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [vat included].
        /// </summary>
        /// <value><c>true</c> if [vat included]; otherwise, <c>false</c>.</value>
        [Required] public bool VATIncluded { get; set; }
    }
}
