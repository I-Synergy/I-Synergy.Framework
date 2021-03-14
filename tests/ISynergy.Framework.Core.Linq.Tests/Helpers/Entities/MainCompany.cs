using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class MainCompany.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    [Table("KendoGrid_MainCompany")]
    public class MainCompany : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the companies.
        /// </summary>
        /// <value>The companies.</value>
        public ICollection<Company> Companies { get; set; }
    }
}