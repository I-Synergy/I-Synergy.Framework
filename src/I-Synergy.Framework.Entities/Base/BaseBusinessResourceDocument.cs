using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.EntityFramework.Entities;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// BaseBusinessResourceDocument model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseBusinessResourceDocument : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the administrative costs.
        /// </summary>
        /// <value>The administrative costs.</value>
        [Required]
        public decimal AdministrativeCosts { get; set; }

        /// <summary>
        /// Gets or sets the administrative costs identifier.
        /// </summary>
        /// <value>The administrative costs identifier.</value>
        [Required]
        public Guid AdministrativeCostsId { get; set; }

        /// <summary>
        /// Gets or sets the administrative costs vat.
        /// </summary>
        /// <value>The administrative costs vat.</value>
        [Required]
        public decimal AdministrativeCostsVAT { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the amount nett.
        /// </summary>
        /// <value>The amount nett.</value>
        [Required]
        public decimal AmountNett { get; set; }

        /// <summary>
        /// Gets or sets the amount vat.
        /// </summary>
        /// <value>The amount vat.</value>
        [Required]
        public decimal AmountVAT { get; set; }

        /// <summary>
        /// Gets or sets the assembly costs.
        /// </summary>
        /// <value>The assembly costs.</value>
        [Required]
        public decimal AssemblyCosts { get; set; }

        /// <summary>
        /// Gets or sets the assembly costs identifier.
        /// </summary>
        /// <value>The assembly costs identifier.</value>
        [Required]
        public Guid AssemblyCostsId { get; set; }

        /// <summary>
        /// Gets or sets the assembly costs vat.
        /// </summary>
        /// <value>The assembly costs vat.</value>
        [Required]
        public decimal AssemblyCostsVAT { get; set; }

        /// <summary>
        /// Gets or sets the billing address addition.
        /// </summary>
        /// <value>The billing address addition.</value>
        [StringLength(3)]
        public string BillingAddressAddition { get; set; }

        /// <summary>
        /// Gets or sets the billing address city.
        /// </summary>
        /// <value>The billing address city.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressCity { get; set; }

        /// <summary>
        /// Gets or sets the billing address country.
        /// </summary>
        /// <value>The billing address country.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressCountry { get; set; }

        /// <summary>
        /// Gets or sets the billing address extra address line.
        /// </summary>
        /// <value>The billing address extra address line.</value>
        [StringLength(255)]
        public string BillingAddressExtraAddressLine { get; set; }

        /// <summary>
        /// Gets or sets the billing address house number.
        /// </summary>
        /// <value>The billing address house number.</value>
        [Required]
        [StringLength(10)]
        public string BillingAddressHouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier.
        /// </summary>
        /// <value>The billing address identifier.</value>
        [Required]
        public Guid BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the state of the billing address.
        /// </summary>
        /// <value>The state of the billing address.</value>
        [StringLength(255)]
        public string BillingAddressState { get; set; }

        /// <summary>
        /// Gets or sets the billing address street.
        /// </summary>
        /// <value>The billing address street.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressStreet { get; set; }

        /// <summary>
        /// Gets or sets the billing address zipcode.
        /// </summary>
        /// <value>The billing address zipcode.</value>
        [Required]
        [StringLength(10)]
        public string BillingAddressZipcode { get; set; }

        /// <summary>
        /// Gets or sets the name of the billing customer.
        /// </summary>
        /// <value>The name of the billing customer.</value>
        [Required]
        [StringLength(255)]
        public string BillingCustomerName { get; set; }

        /// <summary>
        /// Gets or sets the billing contact person title.
        /// </summary>
        /// <value>The billing contact person title.</value>
        [StringLength(255)]
        public string BillingContactPersonTitle { get; set; }

        /// <summary>
        /// Gets or sets the billing contact person salutation.
        /// </summary>
        /// <value>The billing contact person salutation.</value>
        [StringLength(255)]
        public string BillingContactPersonSalutation { get; set; }

        /// <summary>
        /// Gets or sets the billing customer identifier.
        /// </summary>
        /// <value>The billing customer identifier.</value>
        [Required]
        public Guid BillingCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the billing customer number.
        /// </summary>
        /// <value>The billing customer number.</value>
        [Required]
        public int BillingCustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing contact person identifier.
        /// </summary>
        /// <value>The billing contact person identifier.</value>
        public Guid? BillingContactPersonId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>The currency identifier.</value>
        [Required]
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [Required]
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Gets or sets the delivery address addition.
        /// </summary>
        /// <value>The delivery address addition.</value>
        [StringLength(3)]
        public string DeliveryAddressAddition { get; set; }

        /// <summary>
        /// Gets or sets the delivery address city.
        /// </summary>
        /// <value>The delivery address city.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressCity { get; set; }

        /// <summary>
        /// Gets or sets the delivery address country.
        /// </summary>
        /// <value>The delivery address country.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressCountry { get; set; }

        /// <summary>
        /// Gets or sets the delivery address extra address line.
        /// </summary>
        /// <value>The delivery address extra address line.</value>
        [StringLength(255)]
        public string DeliveryAddressExtraAddressLine { get; set; }

        /// <summary>
        /// Gets or sets the delivery address house number.
        /// </summary>
        /// <value>The delivery address house number.</value>
        [Required]
        [StringLength(10)]
        public string DeliveryAddressHouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the delivery address identifier.
        /// </summary>
        /// <value>The delivery address identifier.</value>
        [Required]
        public Guid DeliveryAddressId { get; set; }

        /// <summary>
        /// Gets or sets the state of the delivery address.
        /// </summary>
        /// <value>The state of the delivery address.</value>
        [StringLength(255)]
        public string DeliveryAddressState { get; set; }

        /// <summary>
        /// Gets or sets the delivery address street.
        /// </summary>
        /// <value>The delivery address street.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressStreet { get; set; }

        /// <summary>
        /// Gets or sets the delivery address zipcode.
        /// </summary>
        /// <value>The delivery address zipcode.</value>
        [Required]
        [StringLength(10)]
        public string DeliveryAddressZipcode { get; set; }

        /// <summary>
        /// Gets or sets the delivery condition discount.
        /// </summary>
        /// <value>The delivery condition discount.</value>
        public decimal DeliveryConditionDiscount { get; set; }

        /// <summary>
        /// Gets or sets the delivery condition identifier.
        /// </summary>
        /// <value>The delivery condition identifier.</value>
        [Required]
        public Guid DeliveryConditionId { get; set; }

        /// <summary>
        /// Gets or sets the delivery condition text.
        /// </summary>
        /// <value>The delivery condition text.</value>
        public string DeliveryConditionText { get; set; }

        /// <summary>
        /// Gets or sets the delivery condition value.
        /// </summary>
        /// <value>The delivery condition value.</value>
        public decimal DeliveryConditionValue { get; set; }

        /// <summary>
        /// Gets or sets the delivery costs.
        /// </summary>
        /// <value>The delivery costs.</value>
        [Required]
        public decimal DeliveryCosts { get; set; }

        /// <summary>
        /// Gets or sets the delivery costs identifier.
        /// </summary>
        /// <value>The delivery costs identifier.</value>
        [Required]
        public Guid DeliveryCostsId { get; set; }

        /// <summary>
        /// Gets or sets the delivery costs vat.
        /// </summary>
        /// <value>The delivery costs vat.</value>
        [Required]
        public decimal DeliveryCostsVAT { get; set; }

        /// <summary>
        /// Gets or sets the delivery date.
        /// </summary>
        /// <value>The delivery date.</value>
        public DateTimeOffset? DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the delivery customer.
        /// </summary>
        /// <value>The name of the delivery customer.</value>
        [Required]
        public string DeliveryCustomerName { get; set; }

        /// <summary>
        /// Gets or sets the delivery customer identifier.
        /// </summary>
        /// <value>The delivery customer identifier.</value>
        [Required]
        public Guid DeliveryCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the delivery customer number.
        /// </summary>
        /// <value>The delivery customer number.</value>
        [Required]
        public int DeliveryCustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>The discount.</value>
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
        [Required]
        public DateTimeOffset ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is online.
        /// </summary>
        /// <value><c>true</c> if this instance is online; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>The location identifier.</value>
        public Guid? LocationId { get; set; }

        /// <summary>
        /// Gets or sets the motive identifier.
        /// </summary>
        /// <value>The motive identifier.</value>
        [Required]
        public Guid MotiveId { get; set; }

        /// <summary>
        /// Gets or sets the motive text.
        /// </summary>
        /// <value>The motive text.</value>
        [StringLength(255)]
        public string MotiveText { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        [Required]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the payment condition discount.
        /// </summary>
        /// <value>The payment condition discount.</value>
        public decimal PaymentConditionDiscount { get; set; }

        /// <summary>
        /// Gets or sets the payment condition identifier.
        /// </summary>
        /// <value>The payment condition identifier.</value>
        [Required]
        public Guid PaymentConditionId { get; set; }

        /// <summary>
        /// Gets or sets the payment condition text.
        /// </summary>
        /// <value>The payment condition text.</value>
        public string PaymentConditionText { get; set; }

        /// <summary>
        /// Gets or sets the payment condition value.
        /// </summary>
        /// <value>The payment condition value.</value>
        public decimal PaymentConditionValue { get; set; }

        /// <summary>
        /// Gets or sets the payment term.
        /// </summary>
        /// <value>The payment term.</value>
        [Required]
        public int PaymentTerm { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        [StringLength(255)]
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the servicing product identifier.
        /// </summary>
        /// <value>The servicing product identifier.</value>
        public Guid? ServicingProductId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [StringLength(255)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the vat code identifier.
        /// </summary>
        /// <value>The vat code identifier.</value>
        [Required]
        public Guid VATCodeId { get; set; }

        /// <summary>
        /// Gets or sets the warranty condition discount.
        /// </summary>
        /// <value>The warranty condition discount.</value>
        public decimal WarrantyConditionDiscount { get; set; }

        /// <summary>
        /// Gets or sets the warranty condition identifier.
        /// </summary>
        /// <value>The warranty condition identifier.</value>
        [Required]
        public Guid WarrantyConditionId { get; set; }

        /// <summary>
        /// Gets or sets the warranty condition text.
        /// </summary>
        /// <value>The warranty condition text.</value>
        public string WarrantyConditionText { get; set; }

        /// <summary>
        /// Gets or sets the warranty condition value.
        /// </summary>
        /// <value>The warranty condition value.</value>
        public decimal WarrantyConditionValue { get; set; }

        /// <summary>
        /// Gets or sets the billing email address.
        /// </summary>
        /// <value>The billing email address.</value>
        public string BillingEmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the delivery email address.
        /// </summary>
        /// <value>The delivery email address.</value>
        public string DeliveryEmailAddress { get; set; }
    }
}
