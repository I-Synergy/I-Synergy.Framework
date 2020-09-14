﻿using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class SubFunction : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public long? FunctionId { get; set; }

        public Function Function { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}