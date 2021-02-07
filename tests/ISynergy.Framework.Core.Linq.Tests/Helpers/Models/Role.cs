using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class Role.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// The standard roles
        /// </summary>
        public static readonly Role[] StandardRoles = {
            new Role { Name="Admin", Permissions = new List<Permission> { new Permission { Name = "Admin" } } },
            new Role { Name="User", Permissions = new List<Permission> { new Permission { Name = "User" } } },
            new Role { Name="Guest", Permissions = new List<Permission> { new Permission { Name = "Guest" } } },
            new Role { Name="G", Permissions = new List<Permission> { new Permission { Name = "G" } } },
            new Role { Name="J", Permissions = new List<Permission> { new Permission { Name = "J" } } },
            new Role { Name="A", Permissions = new List<Permission> { new Permission { Name = "A" } } }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <value>The permissions.</value>
        public List<Permission> Permissions { get; set; }
    }
}
