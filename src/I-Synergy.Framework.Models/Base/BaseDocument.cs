using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BaseDocument.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BaseDocument : ModelBase
    {
        /// <summary>
        /// Gets or sets the DateTime property value.
        /// </summary>
        /// <value>The date time.</value>
        [Required]
        public DateTimeOffset DateTime
        {
            get { return GetValue<DateTimeOffset>(); }
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
        /// Gets or sets the FileTypeId property value.
        /// </summary>
        /// <value>The type of the content.</value>
        [Required]
        [StringLength(128)]
        public string ContentType
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Subject property value.
        /// </summary>
        /// <value>The subject.</value>
        [StringLength(255)]
        public string Subject
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Url property value.
        /// </summary>
        /// <value>The URL.</value>
        [Required]
        public string Url
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
