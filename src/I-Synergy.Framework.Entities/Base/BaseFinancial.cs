using ISynergy.Framework.EntityFramework.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// Financial model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseFinancial : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        [Required] [StringLength(3)] public string Currency { get; set; }
        /// <summary>
        /// Gets or sets the credit limit.
        /// </summary>
        /// <value>The credit limit.</value>
        [Required] public decimal CreditLimit { get; set; }
        /// <summary>
        /// Gets or sets the payment condition identifier.
        /// </summary>
        /// <value>The payment condition identifier.</value>
        public Guid PaymentConditionId { get; set; }
        /// <summary>
        /// Gets or sets the payment term.
        /// </summary>
        /// <value>The payment term.</value>
        [Required] public int PaymentTerm { get; set; }
        /// <summary>
        /// Gets or sets the payment condition.
        /// </summary>
        /// <value>The payment condition.</value>
        [StringLength(255)] public string PaymentCondition { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is direct debet.
        /// </summary>
        /// <value><c>true</c> if this instance is direct debet; otherwise, <c>false</c>.</value>
        [Required] public bool IsDirectDebet { get; set; }
        /// <summary>
        /// Gets or sets the maximum direct debet amount.
        /// </summary>
        /// <value>The maximum direct debet amount.</value>
        [Required] public decimal MaximumDirectDebetAmount { get; set; }
        /// <summary>
        /// Gets or sets the invoice discount.
        /// </summary>
        /// <value>The invoice discount.</value>
        [Required] public decimal InvoiceDiscount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is blocked.
        /// </summary>
        /// <value><c>true</c> if this instance is blocked; otherwise, <c>false</c>.</value>
        [Required] public bool IsBlocked { get; set; }
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        [StringLength(255)] public string Reference { get; set; }
    }
}
