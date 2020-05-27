using ISynergy.Models.Enumerations;

namespace ISynergy.Models.Abstractions
{
    /// <summary>
    /// Interface IInternetAddress
    /// </summary>
    public interface IInternetAddress
    {
        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>The type of the address.</value>
        InternetAddressTypes AddressType { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        string Address { get; set; }
    }
}
