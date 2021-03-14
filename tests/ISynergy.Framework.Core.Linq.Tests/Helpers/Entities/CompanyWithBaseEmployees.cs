using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class CompanyWithBaseEmployees.
    /// </summary>
    public class CompanyWithBaseEmployees
    {
        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public ICollection<BaseEmployee> Employees { get; set; }
    }
}
