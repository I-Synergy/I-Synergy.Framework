using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Country : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}