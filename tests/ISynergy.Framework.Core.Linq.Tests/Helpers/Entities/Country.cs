using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Country.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class Country : Entity
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public ICollection<Employee> Employees { get; set; }
    }
}