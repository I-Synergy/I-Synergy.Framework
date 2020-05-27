using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// ContactPerson model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public abstract class BaseContactPerson : BasePerson
    {
        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        [StringLength(255)] public string Function { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        [StringLength(255)] public string Department { get; set; }
        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        [StringLength(128)] public string Phone { get; set; }
        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        /// <value>The mobile.</value>
        [StringLength(128)] public string Mobile { get; set; }
        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>The fax.</value>
        [StringLength(128)] public string Fax { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [StringLength(255)] public string Email { get; set; }
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public int Extension { get; set; }
    }
}
