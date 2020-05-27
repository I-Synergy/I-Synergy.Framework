using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.EntityFramework.Entities;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// BaseBusinessResourceDocumentLine model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseBusinessResourceDocumentLine : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the amount purchase.
        /// </summary>
        /// <value>The amount purchase.</value>
        [Required]
        public decimal AmountPurchase { get; set; }

        /// <summary>
        /// Gets or sets the commodity identifier.
        /// </summary>
        /// <value>The commodity identifier.</value>
        [Required]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// Gets or sets the commodity code.
        /// </summary>
        /// <value>The commodity code.</value>
        [Required]
        [StringLength(255)]
        public string CommodityCode { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>The discount.</value>
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the line identifier.
        /// </summary>
        /// <value>The line identifier.</value>
        [Required]
        [Identity]
        public Guid LineId { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        [Required]
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [StringLength(255)]
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the unit identifier.
        /// </summary>
        /// <value>The unit identifier.</value>
        [Required]
        public Guid UnitId { get; set; }

        /// <summary>
        /// Gets or sets the unit code.
        /// </summary>
        /// <value>The unit code.</value>
        [Required]
        [StringLength(6)]
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the unit description.
        /// </summary>
        /// <value>The unit description.</value>
        [Required]
        [StringLength(50)]
        public string UnitDescription { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [Required]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the vat code identifier.
        /// </summary>
        /// <value>The vat code identifier.</value>
        [Required]
        public Guid VATCodeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [vat included].
        /// </summary>
        /// <value><c>true</c> if [vat included]; otherwise, <c>false</c>.</value>
        [Required]
        public bool VATIncluded { get; set; }

        /// <summary>
        /// Gets or sets the vat percentage.
        /// </summary>
        /// <value>The vat percentage.</value>
        [Required]
        public decimal VATPercentage { get; set; }
    }
}
