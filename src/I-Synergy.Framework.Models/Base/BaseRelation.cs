using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseRelation.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseRelation : ModelBase
    {
        /// <summary>
        /// Gets or sets the Number property value.
        /// </summary>
        /// <value>The number.</value>
        [Required]
        public int Number
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Code property value.
        /// </summary>
        /// <value>The code.</value>
        [Required]
        [StringLength(10)]
        public string Code
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [StringLength(255)]
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsCorporate property value.
        /// </summary>
        /// <value><c>true</c> if this instance is corporate; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsCorporate
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsPrivate property value.
        /// </summary>
        /// <value><c>true</c> if this instance is private; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsPrivate
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsActive property value.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Required]
        public bool IsActive
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

    }
}
