using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BaseCompany.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseCompany : ModelBase
    {
        /// <summary>
        /// Gets or sets the CompanyName property value.
        /// </summary>
        /// <value>The name of the company.</value>
        [Required]
        [StringLength(255)]
        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Trade_Name property value.
        /// </summary>
        /// <value>The trade names.</value>
        [StringLength(255)]
        public string TradeNames
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the VATIdentificationNumber property value.
        /// </summary>
        /// <value>The vat identification number.</value>
        [StringLength(255)]
        public string VATIdentificationNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Number property value.
        /// </summary>
        /// <value>The registration number.</value>
        [StringLength(255)]
        public string RegistrationNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the RegistrationCountryId property value.
        /// </summary>
        /// <value>The registration country.</value>
        [Required]
        [StringLength(255)]
        public string RegistrationCountry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Industry property value.
        /// </summary>
        /// <value>The industry.</value>
        [StringLength(255)]
        public string Industry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LegalForm property value.
        /// </summary>
        /// <value>The legal form.</value>
        [StringLength(255)]
        public string LegalForm
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
