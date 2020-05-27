using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BaseBusinessResourceDocumentBasic.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseBusinessResourceDocumentBasic : ModelBase
    {
        /// <summary>
        /// Gets or sets the CurrencyId property value.
        /// </summary>
        /// <value>The currency identifier.</value>
        [Required]
        public int CurrencyId
        {
            get { return GetValue<int>(); }
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
        /// Gets or sets the ExpirationDate property value.
        /// </summary>
        /// <value>The expiration date.</value>
        [Required]
        public DateTimeOffset ExpirationDate
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCustomerName property value.
        /// </summary>
        /// <value>The name of the delivery customer.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryCustomerName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCustomerNumber property value.
        /// </summary>
        /// <value>The delivery customer number.</value>
        [Required]
        public int DeliveryCustomerNumber
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressStreet property value.
        /// </summary>
        /// <value>The delivery address street.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressStreet
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressExtraAddressLine property value.
        /// </summary>
        /// <value>The delivery address extra address line.</value>
        [StringLength(255)]
        public string DeliveryAddressExtraAddressLine
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressHouseNumber property value.
        /// </summary>
        /// <value>The delivery address house number.</value>
        [Required]
        public string DeliveryAddressHouseNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressAddition property value.
        /// </summary>
        /// <value>The delivery address addition.</value>
        [StringLength(3)]
        public string DeliveryAddressAddition
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressZipcode property value.
        /// </summary>
        /// <value>The delivery address zipcode.</value>
        [Required]
        [StringLength(10)]
        public string DeliveryAddressZipcode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressCity property value.
        /// </summary>
        /// <value>The delivery address city.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressCity
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressState property value.
        /// </summary>
        /// <value>The state of the delivery address.</value>
        [StringLength(255)]
        public string DeliveryAddressState
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryAddressCountry property value.
        /// </summary>
        /// <value>The delivery address country.</value>
        [Required]
        [StringLength(255)]
        public string DeliveryAddressCountry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingCustomerName property value.
        /// </summary>
        /// <value>The name of the billing customer.</value>
        [Required]
        [StringLength(255)]
        public string BillingCustomerName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCustomerNumber property value.
        /// </summary>
        /// <value>The billing customer number.</value>
        [Required]
        public int BillingCustomerNumber
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressStreet property value.
        /// </summary>
        /// <value>The billing address street.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressStreet
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressExtraAddressLine property value.
        /// </summary>
        /// <value>The billing address extra address line.</value>
        [StringLength(255)]
        public string BillingAddressExtraAddressLine
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressHouseNumber property value.
        /// </summary>
        /// <value>The billing address house number.</value>
        [Required]
        public string BillingAddressHouseNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressAddition property value.
        /// </summary>
        /// <value>The billing address addition.</value>
        [StringLength(3)]
        public string BillingAddressAddition
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressZipcode property value.
        /// </summary>
        /// <value>The billing address zipcode.</value>
        [Required]
        [StringLength(10)]
        public string BillingAddressZipcode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressCity property value.
        /// </summary>
        /// <value>The billing address city.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressCity
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressState property value.
        /// </summary>
        /// <value>The state of the billing address.</value>
        [StringLength(255)]
        public string BillingAddressState
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressCountry property value.
        /// </summary>
        /// <value>The billing address country.</value>
        [Required]
        [StringLength(255)]
        public string BillingAddressCountry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingContactPersonTitle property value.
        /// </summary>
        /// <value>The billing contact person title.</value>
        public string BillingContactPersonTitle
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingContactPersonSalutation property value.
        /// </summary>
        /// <value>The billing contact person salutation.</value>
        public string BillingContactPersonSalutation
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Subject property value.
        /// </summary>
        /// <value>The subject.</value>
        [StringLength(255)]
        public string Subject
        {
            get { return GetValue<string>(); }
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

        /// <summary>
        /// Gets or sets the UserId property value.
        /// </summary>
        /// <value>The user identifier.</value>
        [Required]
        public Guid UserId
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
        /// Gets or sets the Number property value.
        /// </summary>
        /// <value>The number.</value>
        public int Number
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingEmailAddress property value.
        /// </summary>
        /// <value>The billing email address.</value>
        public string BillingEmailAddress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryEmailAddress property value.
        /// </summary>
        /// <value>The delivery email address.</value>
        public string DeliveryEmailAddress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public abstract Guid Id { get; }
    }

    /// <summary>
    /// Class BaseBusinessResourceDocumentSearch.
    /// Implements the <see cref="ISynergy.Models.Base.BaseBusinessResourceDocumentBasic" />
    /// </summary>
    /// <seealso cref="ISynergy.Models.Base.BaseBusinessResourceDocumentBasic" />
    public abstract class BaseBusinessResourceDocumentSearch : BaseBusinessResourceDocumentBasic
    {
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
        /// Gets or sets the IsOnline property value.
        /// </summary>
        /// <value><c>true</c> if this instance is online; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsOnline
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the age.
        /// </summary>
        /// <value>The age.</value>
        public int Age
        {
            get
            {
                return Date.ToUniversalTime().Date.AgeInDays();
            }
        }
    }

    /// <summary>
    /// Class BaseBusinessResourceDocumentBase.
    /// Implements the <see cref="ISynergy.Models.Base.BaseBusinessResourceDocumentSearch" />
    /// </summary>
    /// <seealso cref="ISynergy.Models.Base.BaseBusinessResourceDocumentSearch" />
    public abstract class BaseBusinessResourceDocumentBase : BaseBusinessResourceDocumentSearch
    {
        /// <summary>
        /// Gets or sets the AdministrativeCosts property value.
        /// </summary>
        /// <value>The administrative costs.</value>
        [Required]
        public decimal AdministrativeCosts
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AdministrativeCostsId property value.
        /// </summary>
        /// <value>The administrative costs identifier.</value>
        [Required]
        public Guid AdministrativeCostsId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AdministrativeCostsVAT property value.
        /// </summary>
        /// <value>The administrative costs vat.</value>
        [Required]
        public decimal AdministrativeCostsVAT
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AmountNett property value.
        /// </summary>
        /// <value>The amount nett.</value>
        [Required]
        public decimal AmountNett
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AmountVAT property value.
        /// </summary>
        /// <value>The amount vat.</value>
        [Required]
        public decimal AmountVAT
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AssemblyCosts property value.
        /// </summary>
        /// <value>The assembly costs.</value>
        [Required]
        public decimal AssemblyCosts
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AssemblyCostsId property value.
        /// </summary>
        /// <value>The assembly costs identifier.</value>
        [Required]
        public Guid AssemblyCostsId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AssemblyCostsVAT property value.
        /// </summary>
        /// <value>The assembly costs vat.</value>
        [Required]
        public decimal AssemblyCostsVAT
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContactPersonId property value.
        /// </summary>
        /// <value>The billing contact person identifier.</value>
        public Guid BillingContactPersonId
        {
            get { return GetValue<Guid>(); }
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
        /// Gets or sets the DeliveryAddressId property value.
        /// </summary>
        /// <value>The delivery address identifier.</value>
        [Required]
        public Guid DeliveryAddressId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCosts property value.
        /// </summary>
        /// <value>The delivery costs.</value>
        [Required]
        public decimal DeliveryCosts
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCostsId property value.
        /// </summary>
        /// <value>The delivery costs identifier.</value>
        [Required]
        public Guid DeliveryCostsId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCostsVAT property value.
        /// </summary>
        /// <value>The delivery costs vat.</value>
        [Required]
        public decimal DeliveryCostsVAT
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryDate property value.
        /// </summary>
        /// <value>The delivery date.</value>
        public DateTimeOffset? DeliveryDate
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BillingAddressId property value.
        /// </summary>
        /// <value>The billing address identifier.</value>
        [Required]
        public Guid BillingAddressId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LocationId property value.
        /// </summary>
        /// <value>The location identifier.</value>
        public Guid? LocationId
        {
            get { return GetValue<Guid?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the MotiveId property value.
        /// </summary>
        /// <value>The motive identifier.</value>
        [Required]
        public Guid MotiveId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the MotiveText property value.
        /// </summary>
        /// <value>The motive text.</value>
        [StringLength(255)]
        public string MotiveText
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentConditionId property value.
        /// </summary>
        /// <value>The payment condition identifier.</value>
        [Required]
        public Guid PaymentConditionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentConditionValue property value.
        /// </summary>
        /// <value>The payment condition value.</value>
        public decimal PaymentConditionValue
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PaymentConditionText property value.
        /// </summary>
        /// <value>The payment condition text.</value>
        public string PaymentConditionText
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Payment_Condition_Discount property value.
        /// </summary>
        /// <value>The payment condition discount.</value>
        public decimal PaymentConditionDiscount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryConditionId property value.
        /// </summary>
        /// <value>The delivery condition identifier.</value>
        [Required]
        public Guid DeliveryConditionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryConditionValue property value.
        /// </summary>
        /// <value>The delivery condition value.</value>
        public decimal DeliveryConditionValue
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryConditionText property value.
        /// </summary>
        /// <value>The delivery condition text.</value>
        public string DeliveryConditionText
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Delivery_Condition_Discount property value.
        /// </summary>
        /// <value>The delivery condition discount.</value>
        public decimal DeliveryConditionDiscount
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the WarrantyConditionId property value.
        /// </summary>
        /// <value>The warranty condition identifier.</value>
        [Required]
        public Guid WarrantyConditionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the WarrantyConditionValue property value.
        /// </summary>
        /// <value>The warranty condition value.</value>
        public decimal WarrantyConditionValue
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the WarrantyConditionText property value.
        /// </summary>
        /// <value>The warranty condition text.</value>
        public string WarrantyConditionText
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Warranty_Condition_Discount property value.
        /// </summary>
        /// <value>The warranty condition discount.</value>
        public decimal WarrantyConditionDiscount
        {
            get { return GetValue<decimal>(); }
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
        /// Gets or sets the BillingCustomerId property value.
        /// </summary>
        /// <value>The billing customer identifier.</value>
        [Required]
        public Guid BillingCustomerId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DeliveryCustomerId property value.
        /// </summary>
        /// <value>The delivery customer identifier.</value>
        [Required]
        public Guid DeliveryCustomerId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ServicingProductId property value.
        /// </summary>
        /// <value>The servicing product identifier.</value>
        public Guid? ServicingProductId
        {
            get { return GetValue<Guid?>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class BaseBusinessResourceDocument.
    /// Implements the <see cref="ISynergy.Models.Base.BaseBusinessResourceDocumentBase" />
    /// </summary>
    /// <typeparam name="TEntityLine">The type of the t entity line.</typeparam>
    /// <seealso cref="ISynergy.Models.Base.BaseBusinessResourceDocumentBase" />
    public abstract class BaseBusinessResourceDocument<TEntityLine> : BaseBusinessResourceDocumentBase
        where TEntityLine : BaseBusinessResourceDocumentLine, new()
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public override Guid Id => GetValue<Guid>();

        /// <summary>
        /// Gets or sets the Items property value.
        /// </summary>
        /// <value>The details.</value>
        public List<TEntityLine> Details
        {
            get { return GetValue<List<TEntityLine>>(); }
            set { SetValue(value); }
        }
    }
}
