using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Data;
using ISynergy.Models.Enumerations;

namespace ISynergy.Models.Base
{
    /// <summary>
    /// Class BasePhone.
    /// Implements the <see cref="ISynergy.Framework.Core.Data.ModelBase" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Data.ModelBase" />
    public abstract class BasePhone : ModelBase
    {
        /// <summary>
        /// Gets or sets the Phone_Type property value.
        /// </summary>
        /// <value>The type of the phone.</value>
        [Required]
        public PhoneTypes PhoneType
        {
            get { return GetValue<PhoneTypes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the AreaCode property value.
        /// </summary>
        /// <value>The area code.</value>
        [Required]
        [StringLength(5)]
        public string AreaCode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SubscriberNumber property value.
        /// </summary>
        /// <value>The subscriber number.</value>
        [Required]
        [StringLength(10)]
        public string SubscriberNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the CountryId property value.
        /// </summary>
        /// <value>The country prefix.</value>
        [StringLength(128)]
        public string CountryPrefix
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the phonenumber short.
        /// </summary>
        /// <value>The phonenumber short.</value>
        public string PhonenumberShort
        {
            get
            {
                if (AreaCode != null && SubscriberNumber != null)
                {
                    return AreaCode + SubscriberNumber;
                }
                else
                {
                    return string.Empty;
                }
            }
            private set
            {
                return;
            }
        }

        /// <summary>
        /// Gets the phonenumber.
        /// </summary>
        /// <value>The phonenumber.</value>
        public string Phonenumber
        {
            get
            {
                if (CountryPrefix != null && AreaCode != null && SubscriberNumber != null)
                {
                    return $"+{CountryPrefix} {AreaCode.Replace("0", "(0)")} {SubscriberNumber}";
                }
                else
                {
                    return string.Empty;
                }
            }
            private set
            {
                return;
            }
        }
    }
}
