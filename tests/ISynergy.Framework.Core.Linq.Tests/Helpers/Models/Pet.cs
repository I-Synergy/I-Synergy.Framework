
namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class Pet.
    /// </summary>
    public class Pet
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public Person Owner { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>The owner identifier.</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the nullable owner identifier.
        /// </summary>
        /// <value>The nullable owner identifier.</value>
        public int? NullableOwnerId { get; set; }
    }
}
