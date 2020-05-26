using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using Microsoft.AspNetCore.Identity;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    /// <summary>
    /// Class User.
    /// Implements the <see cref="IdentityUser" />
    /// </summary>
    /// <seealso cref="IdentityUser" />
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        [ParentIdentity(typeof(Guid))] [Required] public Guid AccountId { get; set; }
        /// <summary>
        /// Gets or sets the relation identifier.
        /// </summary>
        /// <value>The relation identifier.</value>
        public Guid RelationId { get; set; }
        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        /// <value>The account.</value>
        public Account Account { get; set; }
    }
}
