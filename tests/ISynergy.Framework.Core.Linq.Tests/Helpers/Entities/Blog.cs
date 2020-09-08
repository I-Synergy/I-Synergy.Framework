using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public int? NullableInt { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
