using System;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Locking
{
    /// <summary>
    /// The options for the <see cref="DefaultTimeoutPolicy"/>
    /// </summary>
    public class DefaultTimeoutPolicyOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the timeout may be infinite
        /// </summary>
        public bool AllowInfiniteTimeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum timeout
        /// </summary>
        public TimeSpan? MaxTimeout { get; set; }
    }
}
