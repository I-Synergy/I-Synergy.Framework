using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseFinancial.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public abstract class BaseFinancial : ModelBase
    {
        /// <summary>
        /// Gets or sets the CurrencyId property value.
        /// </summary>
        /// <value>The currency.</value>
        [Required]
        public string Currency
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CreditLimit property value.
        /// </summary>
        /// <value>The credit limit.</value>
        [Required]
        public decimal CreditLimit
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentConditionId property value.
        /// </summary>
        /// <value>The payment condition identifier.</value>
        public Guid PaymentConditionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentTerm property value.
        /// </summary>
        /// <value>The payment term.</value>
        [Required]
        public int PaymentTerm
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentConditionId property value.
        /// </summary>
        /// <value>The payment condition.</value>
        public string PaymentCondition
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsDirectDebet property value.
        /// </summary>
        /// <value><c>true</c> if this instance is direct debet; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsDirectDebet
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the MaximumDirectDebetAmount property value.
        /// </summary>
        /// <value>The maximum direct debet amount.</value>
        [Required]
        public decimal MaximumDirectDebetAmount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the InvoiceDiscount property value.
        /// </summary>
        /// <value>The invoice discount.</value>
        [Required]
        public decimal InvoiceDiscount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsBlocked property value.
        /// </summary>
        /// <value><c>true</c> if this instance is blocked; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsBlocked
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Reference property value.
        /// </summary>
        /// <value>The reference.</value>
        [StringLength(255)]
        public string Reference
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
