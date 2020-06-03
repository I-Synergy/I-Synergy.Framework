using ISynergy.Framework.Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ISynergy.Framework.Core.Models.Accounts
{
    /// <summary>
    /// Class User.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public class User : ModelBase
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty]
        [Required]
        public string Id
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UserName property value.
        /// </summary>
        /// <value>The name of the user.</value>
        [JsonProperty]
        [Required]
        public string UserName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsUnlocked property value.
        /// </summary>
        /// <value><c>true</c> if this instance is unlocked; otherwise, <c>false</c>.</value>
        [JsonProperty]
        [Required]
        public bool IsUnlocked
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class UserSelect.
    /// Implements the <see cref="User" />
    /// </summary>
    /// <seealso cref="User" />
    public class UserSelect : User
    {
        /// <summary>
        /// Gets or sets the IsSelected property value.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class UserAdd.
    /// Implements the <see cref="User" />
    /// </summary>
    /// <seealso cref="User" />
    public class UserAdd : User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAdd"/> class.
        /// </summary>
        public UserAdd()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the AccountId property value.
        /// </summary>
        /// <value>The account identifier.</value>
        [JsonProperty]
        public Guid AccountId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        /// <value>The password.</value>
        [JsonProperty]
        public string Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        /// <value>The roles.</value>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class UserEdit.
    /// Implements the <see cref="User" />
    /// </summary>
    /// <seealso cref="User" />
    public class UserEdit : User
    {
        /// <summary>
        /// Gets or sets the IsConfirmed property value.
        /// </summary>
        /// <value><c>true</c> if this instance is confirmed; otherwise, <c>false</c>.</value>
        [JsonProperty]
        public bool IsConfirmed
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        /// <value>The roles.</value>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }
    }

    /// <summary>
    /// Class UserFull.
    /// Implements the <see cref="User" />
    /// </summary>
    /// <seealso cref="User" />
    public class UserFull : User
    {
        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        /// <value>The roles.</value>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RolesSummary property value.
        /// </summary>
        /// <value>The roles summary.</value>
        public string RolesSummary
        {
            get
            {
                if (Roles != null) return string.Join(", ", Roles.Select(s => s.Name));
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the IsConfirmed property value.
        /// </summary>
        /// <value><c>true</c> if this instance is confirmed; otherwise, <c>false</c>.</value>
        [JsonProperty]
        [Required]
        public bool IsConfirmed
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the FailedAttempts property value.
        /// </summary>
        /// <value>The failed attempts.</value>
        [JsonProperty]
        [Required]
        public int FailedAttempts
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
