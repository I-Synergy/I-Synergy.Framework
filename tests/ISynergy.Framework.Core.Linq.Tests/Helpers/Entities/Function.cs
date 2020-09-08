using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Function : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<SubFunction> SubFunctions { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}