using ISynergy.Models.Enumerations;
using ISynergy.Framework.EntityFramework.Entities;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Bank_Account model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseBankAccount : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the iban.
        /// </summary>
        /// <value>The iban.</value>
        [Required] [StringLength(255)] public string IBAN { get; set; }
        /// <summary>
        /// Gets or sets the bic.
        /// </summary>
        /// <value>The bic.</value>
        [StringLength(255)] public string BIC { get; set; }
        /// <summary>
        /// Gets or sets the ascription.
        /// </summary>
        /// <value>The ascription.</value>
        [Required] [StringLength(255)] public string Ascription { get; set; }
        /// <summary>
        /// Gets or sets the type of the account.
        /// </summary>
        /// <value>The type of the account.</value>
        [Required] public AccountTypes AccountType { get; set; }
    }
}
