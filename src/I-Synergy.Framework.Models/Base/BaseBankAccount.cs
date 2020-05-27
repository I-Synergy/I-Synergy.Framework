using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Models.Enumerations;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseBankAccount.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseBankAccount : ModelBase
    {
        /// <summary>
        /// Gets or sets the IBAN property value.
        /// </summary>
        /// <value>The iban.</value>
        [Required]
        [StringLength(255)]
        public string IBAN
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BIC property value.
        /// </summary>
        /// <value>The bic.</value>
        [StringLength(255)]
        public string BIC
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Ascription property value.
        /// </summary>
        /// <value>The ascription.</value>
        [Required]
        [StringLength(255)]
        public string Ascription
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AccountType property value.
        /// </summary>
        /// <value>The type of the account.</value>
        [Required]
        public AccountTypes AccountType
        {
            get { return GetValue<AccountTypes>(); }
            set { SetValue(value); }
        }
    }
}
