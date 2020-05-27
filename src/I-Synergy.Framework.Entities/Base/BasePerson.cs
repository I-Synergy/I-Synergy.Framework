using ISynergy.Framework.EntityFramework.Entities;
using ISynergy.Models.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Person model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class BasePerson : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the city of birth.
        /// </summary>
        /// <value>The city of birth.</value>
        [StringLength(255)] public string CityOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the country of birth.
        /// </summary>
        /// <value>The country of birth.</value>
        [StringLength(255)] public string CountryOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        public DateTimeOffset? DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [StringLength(255)] public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the history.
        /// </summary>
        /// <value>The history.</value>
        public string History { get; set; }
        /// <summary>
        /// Gets or sets the initials.
        /// </summary>
        /// <value>The initials.</value>
        [StringLength(10)] public string Initials { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is resident.
        /// </summary>
        /// <value><c>true</c> if this instance is resident; otherwise, <c>false</c>.</value>
        public bool IsResident { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [Required] [StringLength(255)] public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        /// <value>The nationality.</value>
        [StringLength(255)] public string Nationality { get; set; }
        /// <summary>
        /// Gets or sets the sex.
        /// </summary>
        /// <value>The sex.</value>
        [Required] public Sexes Sex { get; set; }
        /// <summary>
        /// Gets or sets the social security number.
        /// </summary>
        /// <value>The social security number.</value>
        [StringLength(128)] public string SocialSecurityNumber { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the salutation.
        /// </summary>
        /// <value>The salutation.</value>
        public string Salutation { get; set; }
    }
}
