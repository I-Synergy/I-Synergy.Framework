using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Models.Accounts
{
    public class RegistrationData : ModelBase
    {
        /// <summary>
        /// Gets or sets the ApplicationId property value.
        /// </summary>
        [Required]
        public int ApplicationId
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RelationId property value.
        /// </summary>
        public Guid RelationId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LicenseName property value.
        /// </summary>
        [Required]
        public string LicenseName
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
        /// Gets or sets the Email property value.
        /// </summary>
        [Required]
        public string Email
        {
            get { return GetValue<string>(); }
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

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        [Required]
        public string Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}