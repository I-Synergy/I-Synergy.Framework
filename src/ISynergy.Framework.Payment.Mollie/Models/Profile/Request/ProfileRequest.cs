﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Profile.Request
{
    /// <summary>
    /// Class ProfileRequest.
    /// </summary>
    public class ProfileRequest
    {
        /// <summary>
        /// The profile's name should reflect the tradename or brand name of the profile's website or application.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// The URL to the profile's website or application. The URL should start with http:// or https://.
        /// </summary>
        /// <value>The website.</value>
        public string Website { get; set; }

        /// <summary>
        /// The email address associated with the profile's tradename or brand.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// The phone number associated with the profile's tradename or brand.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone { get; set; }

        /// <summary>
        /// The industry associated with the profile's tradename or brand.
        /// </summary>
        /// <value>The category code.</value>
        public CategoryCode CategoryCode { get; set; }

        /// <summary>
        /// Optional – Creating a test profile by setting this parameter to test, enables you to start using the API without
        /// having to provide all your business info just yet. Defaults to live.
        /// </summary>
        /// <value>The mode.</value>
        public Mode? Mode { get; set; }
    }
}
