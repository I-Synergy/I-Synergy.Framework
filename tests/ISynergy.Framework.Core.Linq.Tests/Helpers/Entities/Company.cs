using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Company.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class Company : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the main company identifier.
        /// </summary>
        /// <value>The main company identifier.</value>
        public long? MainCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the main company.
        /// </summary>
        /// <value>The main company.</value>
        public MainCompany MainCompany { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public ICollection<Employee> Employees { get; set; }
    }
}