namespace ISynergy.Framework.Core.Abstractions
{
    /// <summary>
    /// Interface IAddress
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>The street.</value>
        string Street { get; set; }
        /// <summary>
        /// Gets or sets the extra address line.
        /// </summary>
        /// <value>The extra address line.</value>
        string ExtraAddressLine { get; set; }
        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>The house number.</value>
        int HouseNumber { get; set; }
        /// <summary>
        /// Gets or sets the addition.
        /// </summary>
        /// <value>The addition.</value>
        string Addition { get; set; }
        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>The zipcode.</value>
        string Zipcode { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        string City { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        string State { get; set; }
        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>The country identifier.</value>
        int CountryId { get; set; }
        /// <summary>
        /// Gets or sets the address type identifier.
        /// </summary>
        /// <value>The address type identifier.</value>
        int AddressTypeId { get; set; }
    }
}
