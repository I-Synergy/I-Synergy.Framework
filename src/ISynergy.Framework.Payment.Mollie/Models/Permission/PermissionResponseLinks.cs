using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Permission
{
    /// <summary>
    /// Class PermissionResponseLinks.
    /// </summary>
    public class PermissionResponseLinks
    {
        /// <summary>
        /// The API resource URL of the permission itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<PermissionResponse> Self { get; set; }

        /// <summary>
        /// The URL to the permission retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
