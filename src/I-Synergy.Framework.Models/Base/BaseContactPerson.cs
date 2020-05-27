using System.ComponentModel.DataAnnotations;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BaseContactPerson.
    /// Implements the <see cref="ISynergy.Models.Base.BasePerson" />
    /// </summary>
    /// <seealso cref="ISynergy.Models.Base.BasePerson" />
    public abstract class BaseContactPerson : BasePerson
    {
        /// <summary>
        /// Gets or sets the Function property value.
        /// </summary>
        /// <value>The function.</value>
        [StringLength(255)]
        public string Function
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Department property value.
        /// </summary>
        /// <value>The department.</value>
        [StringLength(255)]
        public string Department
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Phone property value.
        /// </summary>
        /// <value>The phone.</value>
        [StringLength(128)]
        public string Phone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mobile property value.
        /// </summary>
        /// <value>The mobile.</value>
        [StringLength(128)]
        public string Mobile
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Fax property value.
        /// </summary>
        /// <value>The fax.</value>
        [StringLength(128)]
        public string Fax
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Email property value.
        /// </summary>
        /// <value>The email.</value>
        [StringLength(255)]
        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Extension property value.
        /// </summary>
        /// <value>The extension.</value>
        public int Extension
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
