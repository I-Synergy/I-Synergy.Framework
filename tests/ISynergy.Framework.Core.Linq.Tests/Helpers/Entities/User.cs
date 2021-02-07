using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class User.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class User : Entity
    {
        /// <summary>
        /// Gets or sets the name of the identity.
        /// </summary>
        /// <value>The name of the identity.</value>
        public string IdentityName { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public ICollection<Role> Roles { get; set; }

        /// <summary>
        /// Determines whether the specified role has role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns><c>true</c> if the specified role has role; otherwise, <c>false</c>.</returns>
        public bool HasRole(string role)
        {
            return Roles.Any(r => r.Name == role);
        }
    }
}
