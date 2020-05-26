using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class CompanyWithBaseEmployees
    {
        public ICollection<BaseEmployee> Employees { get; set; }
    }
}
