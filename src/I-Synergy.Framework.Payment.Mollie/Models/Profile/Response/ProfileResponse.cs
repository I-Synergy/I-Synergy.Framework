using System;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Profile.Response
{
    /// <summary>
    /// Class ProfileResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class ProfileResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a profile object. Will always contain profile for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The identifier uniquely referring to this profile, for example pfl_v9hTwCvYqw.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Indicates whether the payment profile is in test or production mode.
        /// Possible values: live or test
        /// </summary>
        /// <value>The mode.</value>
        public Mode Mode { get; set; }

        /// <summary>
        /// The payment profile's name, this will usually reflect the tradename or brand name of the profile's website or
        /// application.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// The URL to the profile's website or application.
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
        /// The profile status determines whether the payment profile is able to receive live payments.
        /// </summary>
        /// <value>The status.</value>
        public ProfileStatus Status { get; set; }

        /// <summary>
        /// The presence of a review object indicates changes have been made that have not yet been approved by ISynergy.Framework.Payment.Mollie.
        /// Changes to test profiles are approved automatically, unless a switch to a live profile has been requested.
        /// The review object will therefore usually be null in test mode.
        /// </summary>
        /// <value>The review.</value>
        public Review Review { get; set; }

        /// <summary>
        /// The payment profile's date and time of creation.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Useful URLs to related resources.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public ProfileResponseLinks Links { get; set; }
    }
}
