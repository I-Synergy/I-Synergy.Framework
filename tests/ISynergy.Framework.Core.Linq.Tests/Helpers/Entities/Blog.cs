using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Blog.
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// Gets or sets the blog identifier.
        /// </summary>
        /// <value>The blog identifier.</value>
        public int BlogId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the nullable int.
        /// </summary>
        /// <value>The nullable int.</value>
        public int? NullableInt { get; set; }
        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        /// <value>The posts.</value>
        public virtual ICollection<Post> Posts { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTimeOffset Created { get; set; }
    }
}
