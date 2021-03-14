using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Function.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class Function : Entity
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
        /// Gets or sets the sub functions.
        /// </summary>
        /// <value>The sub functions.</value>
        public ICollection<SubFunction> SubFunctions { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public ICollection<Employee> Employees { get; set; }
    }
}