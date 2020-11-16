using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Models.Abstractions;
using ISynergy.Framework.Models.Enumerations;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseAddress.
    /// Implements the <see cref="ModelBase" />
    /// Implements the <see cref="IAddress" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    /// <seealso cref="IAddress" />
    public abstract class BaseAddress : ModelBase, IAddress
    {
        /// <summary>
        /// Gets or sets the Address_Type property value.
        /// </summary>
        /// <value>The type of the address.</value>
        [Required]
        public AddressTypes AddressType
        {
            get { return GetValue<AddressTypes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Street property value.
        /// </summary>
        /// <value>The street.</value>
        [Required]
        [StringLength(255)]
        public string Street
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ExtraAddressLine property value.
        /// </summary>
        /// <value>The extra address line.</value>
        [StringLength(255)]
        public string ExtraAddressLine
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the HouseNumber property value.
        /// </summary>
        /// <value>The house number.</value>
        [Required]
        public int HouseNumber
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Addition property value.
        /// </summary>
        /// <value>The addition.</value>
        [StringLength(3)]
        public string Addition
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Zipcode property value.
        /// </summary>
        /// <value>The zipcode.</value>
        [StringLength(10)]
        [Required]
        public string Zipcode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the City property value.
        /// </summary>
        /// <value>The city.</value>
        [StringLength(255)]
        [Required]
        public string City
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the State property value.
        /// </summary>
        /// <value>The state.</value>
        [StringLength(255)]
        public string State
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Country property value.
        /// </summary>
        /// <value>The country.</value>
        [Required]
        [StringLength(255)]
        public string Country
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsDefault property value.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the address short.
        /// </summary>
        /// <value>The address short.</value>
        public string AddressShort() =>
            $"{Street} {HouseNumber}{(Addition != null ? Addition.ToUpper() : string.Empty)}" +
            $"{(ExtraAddressLine != null ? " " + ExtraAddressLine : string.Empty)}"
            .Trim();

        /// <summary>
        /// Gets the address long.
        /// </summary>
        /// <value>The address long.</value>
        public string AddressLong() =>
            $"{Street} {HouseNumber}{(Addition != null ? Addition.ToUpper() : string.Empty)}" +
            $"{(ExtraAddressLine != null ? " " + ExtraAddressLine : string.Empty)}"
            .Trim() + $", {Zipcode} {City}"; 
    }
}
