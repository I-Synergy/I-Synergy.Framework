using ISynergy.Framework.Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ISynergy.Framework.Core.Models.Accounts
{
    public class User : ModelBase
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
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
        [JsonProperty]
        [Required]
        public bool IsUnlocked
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }

    public class Role : ModelBase
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        [JsonProperty]
        [Required]
        public string Id
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        [JsonProperty]
        [Required]
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        [JsonIgnore]
        public string Description { get; set; } = string.Empty;
    }

    public class UserSelect : User
    {
        /// <summary>
        /// Gets or sets the IsSelected property value.
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }

    public class UserAdd : User
    {
        public UserAdd()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the AccountId property value.
        /// </summary>
        [JsonProperty]
        public Guid AccountId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        [JsonProperty]
        public string Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }
    }

    public class UserEdit : User
    {
        /// <summary>
        /// Gets or sets the IsConfirmed property value.
        /// </summary>
        [JsonProperty]
        public bool IsConfirmed
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }
    }

    public class UserFull : User
    {
        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        [JsonProperty]
        public List<Role> Roles
        {
            get { return GetValue<List<Role>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RolesSummary property value.
        /// </summary>
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
        [JsonProperty]
        [Required]
        public int FailedAttempts
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
