
using System;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Post.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Gets or sets the post identifier.
        /// </summary>
        /// <value>The post identifier.</value>
        public int PostId { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the blog identifier.
        /// </summary>
        /// <value>The blog identifier.</value>
        public int BlogId { get; set; }
        /// <summary>
        /// Gets or sets the blog.
        /// </summary>
        /// <value>The blog.</value>
        public virtual Blog Blog { get; set; }

        /// <summary>
        /// Gets or sets the number of reads.
        /// </summary>
        /// <value>The number of reads.</value>
        public int NumberOfReads { get; set; }

        /// <summary>
        /// Gets or sets the post date.
        /// </summary>
        /// <value>The post date.</value>
        public DateTime? PostDate { get; set; }
    }
}
