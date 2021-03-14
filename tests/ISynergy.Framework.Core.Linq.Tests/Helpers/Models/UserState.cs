using System;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class UserState.
    /// </summary>
    public class UserState
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public Guid StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UserState"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Guid(UserState state) => state?.StatusCode ?? Guid.Empty;
    }
}
