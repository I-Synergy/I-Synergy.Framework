using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;
using ISynergy.Models.Abstractions;

namespace ISynergy.Models.Modules
{
    /// <summary>
    /// Class Caller.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// Implements the <see cref="ISynergy.Models.Abstractions.ICaller" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// <seealso cref="ISynergy.Models.Abstractions.ICaller" />
    public class Caller : ModelBase, ICaller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Caller"/> class.
        /// </summary>
        public Caller() { Caller_Id = Guid.NewGuid(); }

        /// <summary>
        /// Gets or sets the Caller_Id property value.
        /// </summary>
        /// <value>The caller identifier.</value>
        [Identity]
        [Required]
        public Guid Caller_Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

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
        /// Gets or sets the PhoneNumber property value.
        /// </summary>
        /// <value>The phone number.</value>
        [Required]
        [StringLength(25)]
        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Relation property value.
        /// </summary>
        /// <value>The relation.</value>
        [StringLength(255)]
        public string Relation
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
