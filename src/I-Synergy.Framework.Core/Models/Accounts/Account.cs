using ISynergy.Framework.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts
{
    public class Account : ModelBase
    {
        public Account() { AccountId = Guid.NewGuid(); }

        /// <summary>
        /// Gets or sets the Account_Id property value.
        /// </summary>
        [Required]
        public Guid AccountId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CustomerId property value.
        /// </summary>
        public Guid RelationId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
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
        [Required]
        public List<Module> Modules
        {
            get { return GetValue<List<Module>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UsersAllowed property value.
        /// </summary>
        [Required]
        public int UsersAllowed
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RegistrationDate property value.
        /// </summary>
        [Required]
        public DateTimeOffset RegistrationDate
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ExpirationDate property value.
        /// </summary>
        [Required]
        public DateTimeOffset ExpirationDate
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsActive property value.
        /// </summary>
        [Required]
        public bool IsActive
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary> 
        /// Gets or sets the TimeZoneId property value. 
        /// </summary> 
        [Required]
        public string TimeZoneId
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }

    public class AccountFull : Account
    {
        /// <summary>
        /// Gets or sets the Users property value.
        /// </summary>
        public List<UserFull> Users
        {
            get { return GetValue<List<UserFull>>(); }
            set { SetValue(value); }
        }
    }
}
