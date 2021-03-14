namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class UserProfile.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the user profile details.
        /// </summary>
        /// <value>The user profile details.</value>
        public UserProfileDetails UserProfileDetails { get; set; }
    }
}
