using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Role : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}