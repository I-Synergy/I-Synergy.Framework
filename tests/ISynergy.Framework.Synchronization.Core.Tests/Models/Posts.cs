using System.Collections.Generic;

namespace ISynergy.Framework.Synchronization.Core.Tests.Models
{
    public partial class Posts
    {
        public Posts()
        {
            PostTag = new HashSet<PostTag>();
        }

        public int PostId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<PostTag> PostTag { get; set; }
    }
}
