using ISynergy.Framework.Models.Enumerations;
using ISynergy.Framework.EntityFramework.Entities;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// Class BaseAddress.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BaseAddress : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>The type of the address.</value>
        [Required] public AddressTypes AddressType { get; set; }
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>The street.</value>
        [Required] [StringLength(255)] public string Street { get; set; }
        /// <summary>
        /// Gets or sets the extra address line.
        /// </summary>
        /// <value>The extra address line.</value>
        [StringLength(255)] public string ExtraAddressLine { get; set; }
        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>The house number.</value>
        [Required] public int HouseNumber { get; set; }
        /// <summary>
        /// Gets or sets the addition.
        /// </summary>
        /// <value>The addition.</value>
        [StringLength(3)] public string Addition { get; set; }
        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>The zipcode.</value>
        [StringLength(10)] public string Zipcode { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        [StringLength(255)] [Required] public string City { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        [StringLength(255)] public string State { get; set; }
        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [Required] [StringLength(255)] public string Country { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault { get; set; }
    }
}
