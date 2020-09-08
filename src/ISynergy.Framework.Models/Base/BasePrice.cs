using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BasePrice.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public abstract class BasePrice : ModelBase
    {
        /// <summary>
        /// Gets or sets the Units property value.
        /// </summary>
        /// <value>The units.</value>
        [Required]
        public decimal Units
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UnitsPerPacking property value.
        /// </summary>
        /// <value>The units per packing.</value>
        [Required]
        public decimal UnitsPerPacking
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Amount property value.
        /// </summary>
        /// <value>The amount.</value>
        [Required]
        public decimal Amount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the VATCodeId property value.
        /// </summary>
        /// <value>The vat code identifier.</value>
        [Required]
        public Guid VATCodeId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the VATCodeId property value.
        /// </summary>
        /// <value>The vat percentage.</value>
        [Required]
        public decimal VATPercentage
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the VATIncluded property value.
        /// </summary>
        /// <value><c>true</c> if [vat included]; otherwise, <c>false</c>.</value>
        [Required]
        public bool VATIncluded
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UnitId property value.
        /// </summary>
        /// <value>The unit identifier.</value>
        [Required]
        public Guid UnitId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UnitCode property value.
        /// </summary>
        /// <value>The unit code.</value>
        [Required]
        [StringLength(6)]
        public string UnitCode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UnitDescription property value.
        /// </summary>
        /// <value>The unit description.</value>
        [Required]
        [StringLength(50)]
        public string UnitDescription
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return $"{UnitCode} - {UnitDescription} {Amount:C2}";
            }
        }
    }
}
