using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Role.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class Role : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public ICollection<User> Users { get; set; }
    }
}