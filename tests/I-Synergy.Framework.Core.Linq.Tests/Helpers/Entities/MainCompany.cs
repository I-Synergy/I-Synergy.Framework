using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    [Table("KendoGrid_MainCompany")]
    public class MainCompany : Entity
    {
        public string Name { get; set; }

        public ICollection<Company> Companies { get; set; }
    }
}