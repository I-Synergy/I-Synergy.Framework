
namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class Person.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the nullable identifier.
        /// </summary>
        /// <value>The nullable identifier.</value>
        public int? NullableId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>
    /// Class PersonAge.
    /// </summary>
    public class PersonAge
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        public int Age { get; set; }
    }
}
