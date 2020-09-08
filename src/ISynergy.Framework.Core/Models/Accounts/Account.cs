using ISynergy.Framework.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts
{
    /// <summary>
    /// Class Account.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public class Account : ModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account() { AccountId = Guid.NewGuid(); }

        /// <summary>
        /// Gets or sets the Account_Id property value.
        /// </summary>
        /// <value>The account identifier.</value>
        [Required]
        public Guid AccountId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CustomerId property value.
        /// </summary>
        /// <value>The relation identifier.</value>
        public Guid RelationId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [StringLength(128)]
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Modules property value.
        /// </summary>
        /// <value>The modules.</value>
        [Required]
        public List<Module> Modules
        {
            get { return GetValue<List<Module>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UsersAllowed property value.
        /// </summary>
        /// <value>The users allowed.</value>
        [Required]
        public int UsersAllowed
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RegistrationDate property value.
        /// </summary>
        /// <value>The registration date.</value>
        [Required]
        public DateTimeOffset RegistrationDate
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
        /// Gets or sets the IsActive property value.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsActive
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TimeZoneId property value.
        /// </summary>
        /// <value>The time zone identifier.</value>
        [Required]
        public string TimeZoneId
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class AccountFull.
    /// Implements the <see cref="Account" />
    /// </summary>
    /// <seealso cref="Account" />
    public class AccountFull : Account
    {
        /// <summary>
        /// Gets or sets the Users property value.
        /// </summary>
        /// <value>The users.</value>
        public List<UserFull> Users
        {
            get { return GetValue<List<UserFull>>(); }
            set { SetValue(value); }
        }
    }
}
