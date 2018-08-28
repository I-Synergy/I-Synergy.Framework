using ISynergy.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Models.Accounts
{
    public class Account : BaseModel
    {
        public Account() { Account_Id = Guid.NewGuid(); }

        /// <summary>
        /// Gets or sets the Account_Id property value.
        /// </summary>
        [Required]
        public Guid Account_Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Relation_Id property value.
        /// </summary>
        public Guid Relation_Id
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
        public DateTimeOffset Registration_Date
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ExpirationDate property value.
        /// </summary>
        [Required]
        public DateTimeOffset Expiration_Date
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