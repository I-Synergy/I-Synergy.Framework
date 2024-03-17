using ISynergy.Framework.AspNetCore.WebDav.Server;

namespace ISynergy.Framework.AspNetCore.WebDav.Options
{
    /// <summary>
    /// Options for the WebDAV host
    /// </summary>
    public class WebDavHostOptions
    {
        /// <summary>
        /// Gets or sets the base URL of the WebDAV server
        /// </summary>
        /// <remarks>
        /// This is usually required when run behind a proxy server. When it is set,
        /// then it must point to the <see cref="IWebDavContext.ServiceBaseUrl"/>!
        /// </remarks>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous WebDAV access is allowed.
        /// </summary>
        public bool AllowAnonymousAccess { get; set; }

        /// <summary>
        /// Gets or sets the home path for the unauthenticated user.
        /// </summary>
        public string AnonymousHomePath { get; set; }
    }
}
