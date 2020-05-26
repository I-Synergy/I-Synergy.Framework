using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    /// <summary>
    /// Class Account.
    /// Implements the <see cref="ClassBase" />
    /// </summary>
    /// <seealso cref="ClassBase" />
    public class Account : ClassBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
            ApiKeys = new HashSet<ApiKey>();
            AccountModules = new HashSet<AccountModule>();
            Users = new HashSet<User>();
        }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        [Required] [Identity] public Guid AccountId { get; set; }
        /// <summary>
        /// Gets or sets the relation identifier.
        /// </summary>
        /// <value>The relation identifier.</value>
        [Required] public Guid RelationId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required] [StringLength(128)] public string Description { get; set; }
        /// <summary>
        /// Gets or sets the users allowed.
        /// </summary>
        /// <value>The users allowed.</value>
        [Required] public int UsersAllowed { get; set; }
        /// <summary>
        /// Gets or sets the registration date.
        /// </summary>
        /// <value>The registration date.</value>
        [Required] public DateTimeOffset RegistrationDate { get; set; }
        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
        [Required] public DateTimeOffset ExpirationDate { get; set; }
        /// <summary>
        /// Gets or sets the time zone identifier.
        /// </summary>
        /// <value>The time zone identifier.</value>
        [Required] public string TimeZoneId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Required] public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTimeOffset CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the changed date.
        /// </summary>
        /// <value>The changed date.</value>
        public DateTimeOffset? ChangedDate { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the changed by.
        /// </summary>
        /// <value>The changed by.</value>
        public string ChangedBy { get; set; }

        /// <summary>
        /// Gets or sets the API keys.
        /// </summary>
        /// <value>The API keys.</value>
        public ICollection<ApiKey> ApiKeys { get; set; }
        /// <summary>
        /// Gets or sets the account modules.
        /// </summary>
        /// <value>The account modules.</value>
        public ICollection<AccountModule> AccountModules { get; set; }
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public ICollection<User> Users { get; set; }
    }
}
