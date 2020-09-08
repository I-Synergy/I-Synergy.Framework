namespace ISynergy.Framework.Models.Abstractions
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
        public string Street { get; set; }
        /// <summary>
        /// Gets or sets the extra address line.
        /// </summary>
        /// <value>The extra address line.</value>
        public string ExtraAddressLine { get; set; }
        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>The house number.</value>
        public int HouseNumber { get; set; }
        /// <summary>
        /// Gets or sets the addition.
        /// </summary>
        /// <value>The addition.</value>
        public string Addition { get; set; }
        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>The zipcode.</value>
        public string Zipcode { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; set; }
        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; }
    }
}
