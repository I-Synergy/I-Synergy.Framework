using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Models.Enumerations;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BaseInternetAddress.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseInternetAddress : ModelBase
    {
        /// <summary>
        /// Gets or sets the Address_Type property value.
        /// </summary>
        /// <value>The type of the address.</value>
        [Required]
        public InternetAddressTypes AddressType
        {
            get { return GetValue<InternetAddressTypes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Address property value.
        /// </summary>
        /// <value>The address.</value>
        [Required]
        [StringLength(255)]
        public string Address
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
