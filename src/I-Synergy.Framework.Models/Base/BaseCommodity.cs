using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseCommodity.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public class BaseCommodity : ModelBase
    {
        /// <summary>
        /// Gets or sets the Code property value.
        /// </summary>
        /// <value>The code.</value>
        [Required]
        [StringLength(255)]
        public string Code
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description_Long property value.
        /// </summary>
        /// <value>The description long.</value>
        public string DescriptionLong
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DescriptionShort property value.
        /// </summary>
        /// <value>The description short.</value>
        [Required]
        [StringLength(255)]
        public string DescriptionShort
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
