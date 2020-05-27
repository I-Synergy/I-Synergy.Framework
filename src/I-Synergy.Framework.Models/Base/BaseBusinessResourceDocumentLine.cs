using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Financial;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BaseBusinessResourceDocumentLineBase.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseBusinessResourceDocumentLineBase : ModelBase
    {
        /// <summary>
        /// Gets or sets the ParentId property value.
        /// </summary>
        /// <param name="value">The value.</param>
        public abstract void ParentId(Guid value);

        /// <summary>
        /// Calculates the totals.
        /// </summary>
        public void CalculateTotals()
        {
            Total = Amount * Quantity; //- (Discount * Quantity);

            if (VATIncluded)
            {
                TotalVATIncluded = Amount * Quantity; // - (Discount * Quantity);
            }
            else
            {
                TotalVATIncluded = VAT.CalculateAmountFromAmountIncludingVAT(VATPercentage, Amount * Quantity);
            }

            RetailPrice = Amount + Discount;
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
        /// Gets or sets the AmountPurchase property value.
        /// </summary>
        /// <value>The amount purchase.</value>
        [Required]
        public decimal AmountPurchase
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CommodityId property value.
        /// </summary>
        /// <value>The commodity identifier.</value>
        [Required]
        public Guid CommodityId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Quantity property value.
        /// </summary>
        /// <value>The quantity.</value>
        [Required]
        public decimal Quantity
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [StringLength(255)]
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Discount property value.
        /// </summary>
        /// <value>The discount.</value>
        [Required]
        public decimal Discount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LineId property value.
        /// </summary>
        /// <value>The line identifier.</value>
        [Identity]
        [Required]
        public Guid LineId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LineNumber property value.
        /// </summary>
        /// <value>The line number.</value>
        [Required]
        public int LineNumber
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Note property value.
        /// </summary>
        /// <value>The note.</value>
        public string Note
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Position property value.
        /// </summary>
        /// <value>The position.</value>
        [StringLength(255)]
        public string Position
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PriceId property value.
        /// </summary>
        /// <value>The price identifier.</value>
        [Required]
        public Guid PriceId
        {
            get { return GetValue<Guid>(); }
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
        /// Gets or sets the Total property value.
        /// </summary>
        /// <value>The total.</value>
        public decimal Total
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TotalVATIncluded property value.
        /// </summary>
        /// <value>The total vat included.</value>
        public decimal TotalVATIncluded
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RetailPrice property value.
        /// </summary>
        /// <value>The retail price.</value>
        public decimal RetailPrice
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CommodityCode property value.
        /// </summary>
        /// <value>The commodity code.</value>
        [Required]
        [StringLength(255)]
        public string CommodityCode
        {
            get { return GetValue<string>(); }
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
        /// Gets or sets the VATPercentage property value.
        /// </summary>
        /// <value>The vat percentage.</value>
        [Required]
        public decimal VATPercentage
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Date property value.
        /// </summary>
        /// <value>The date.</value>
        [Required]
        public DateTimeOffset Date
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Number property value.
        /// </summary>
        /// <value>The number.</value>
        [Required]
        public int Number
        {
            get { return GetValue<int>(); }
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
    }

    /// <summary>
    /// Class BaseBusinessResourceDocumentLine.
    /// Implements the <see cref="ISynergy.Models.Base.BaseBusinessResourceDocumentLineBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Models.Base.BaseBusinessResourceDocumentLineBase" />
    public abstract class BaseBusinessResourceDocumentLine : BaseBusinessResourceDocumentLineBase
    {
    }
}
