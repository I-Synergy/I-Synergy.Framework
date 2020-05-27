using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Models.Enumerations;

namespace ISynergy.Framework.Models.Base
{
    /// <summary>
    /// Class BasePerson.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public class BasePerson : ModelBase
    {
        /// <summary>
        /// Gets or sets the City_Of_Birth property value.
        /// </summary>
        /// <value>The city of birth.</value>
        [StringLength(255)]
        public string CityOfBirth
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Country_Of_Birth property value.
        /// </summary>
        /// <value>The country of birth.</value>
        [StringLength(255)]
        public string CountryOfBirth
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Date_Of_Birth property value.
        /// </summary>
        /// <value>The date of birth.</value>
        public DateTimeOffset? DateOfBirth
        {
            get { return GetValue<DateTimeOffset?>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the First_Name property value.
        /// </summary>
        /// <value>The first name.</value>
        [StringLength(255)]
        public string FirstName
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                SetInitials();
                SetFullName();
            }
        }

        /// <summary>
        /// Gets or sets the LastName property value.
        /// </summary>
        /// <value>The last name.</value>
        [Required]
        [StringLength(255)]
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); SetFullName(); }
        }

        private void SetFullName()
        {
            if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName)) FullName = $"{LastName}, {FirstName}";
        }

        private void SetInitials()
        {
            if (!string.IsNullOrEmpty(FirstName)) Initials = $"{FirstName.Substring(0, 1)}.";
        }

        /// <summary>
        /// Gets or sets the History property value.
        /// </summary>
        /// <value>The history.</value>
        public string History
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Initials property value.
        /// </summary>
        /// <value>The initials.</value>
        [StringLength(10)]
        public string Initials
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsResident property value.
        /// </summary>
        /// <value><c>true</c> if this instance is resident; otherwise, <c>false</c>.</value>
        public bool IsResident
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Nationality property value.
        /// </summary>
        /// <value>The nationality.</value>
        [StringLength(255)]
        public string Nationality
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Sex_Id property value.
        /// </summary>
        /// <value>The sex.</value>
        [Required]
        public Sexes Sex
        {
            get { return GetValue<Sexes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SSN property value.
        /// </summary>
        /// <value>The social security number.</value>
        [StringLength(128)]
        public string SocialSecurityNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TitleId property value.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Salutation property value.
        /// </summary>
        /// <value>The salutation.</value>
        public string Salutation
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the FullName property value.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName
        {
            get { return GetValue<string>(); }
            internal set { SetValue(value); }
        }
    }
}
