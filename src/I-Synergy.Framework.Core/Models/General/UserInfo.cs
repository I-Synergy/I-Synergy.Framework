using ISynergy.Models.Base;
using System;
using System.Collections.Generic;

namespace ISynergy.Models.General
{
    public class AuthInfo : ModelBase
    {
        /// <summary>
        /// Gets or sets the Modules property value.
        /// </summary>
        public List<string> Modules
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        public List<string> Roles
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        public AuthInfo(List<string> modules, List<string> roles)
        {
            Modules = modules;
            Roles = roles;
        }
    }

    /// <summary>
    /// UserInfo model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class UserInfo : ModelBase
    {
        /// <summary>
        /// Gets or sets the Account_Id property value.
        /// </summary>
        public Guid Account_Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the Account_Description property value.
        /// </summary>
        public string Account_Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TimeZoneId property value.
        /// </summary>
        public string TimeZoneId
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the User_Id property value.
        /// </summary>
        public Guid User_Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Uid property value.
        /// </summary>
        public int RfidUid
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        public string Username
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Email property value.
        /// </summary>
        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        public List<string> Roles
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Modules property value.
        /// </summary>
        public List<string> Modules
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the License_Expration property value.
        /// </summary>
        public DateTimeOffset License_Expration
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the License_Users property value.
        /// </summary>
        public int License_Users
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}