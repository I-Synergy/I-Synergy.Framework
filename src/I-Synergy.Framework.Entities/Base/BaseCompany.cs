using ISynergy.Framework.EntityFramework.Entities;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// Class BaseCompany.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BaseCompany : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        [Required] [StringLength(255)] public string CompanyName { get; set; }
        /// <summary>
        /// Gets or sets the trade names.
        /// </summary>
        /// <value>The trade names.</value>
        [StringLength(255)] public string TradeNames { get; set; }
        /// <summary>
        /// Gets or sets the industry.
        /// </summary>
        /// <value>The industry.</value>
        [StringLength(255)] public string Industry { get; set; }
        /// <summary>
        /// Gets or sets the legal form.
        /// </summary>
        /// <value>The legal form.</value>
        [StringLength(255)] public string LegalForm { get; set; }
        /// <summary>
        /// Gets or sets the vat identification number.
        /// </summary>
        /// <value>The vat identification number.</value>
        [StringLength(255)] public string VATIdentificationNumber { get; set; }
        /// <summary>
        /// Gets or sets the registration number.
        /// </summary>
        /// <value>The registration number.</value>
        [StringLength(255)] public string RegistrationNumber { get; set; }
        /// <summary>
        /// Gets or sets the registration country.
        /// </summary>
        /// <value>The registration country.</value>
        [Required] [StringLength(255)] public string RegistrationCountry { get; set; }
    }
}
