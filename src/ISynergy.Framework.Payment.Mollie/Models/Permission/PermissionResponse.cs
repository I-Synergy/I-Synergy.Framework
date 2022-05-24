using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Permission
{
    /// <summary>
    /// Class PermissionResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class PermissionResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a permission object.
        /// Possible values: permission
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The permission's identifier. See OAuth: Permissions for more details about the available permissions.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// A short description of what the permission allows.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Whether this permission is granted to the app by the organization or not.
        /// </summary>
        /// <value><c>true</c> if granted; otherwise, <c>false</c>.</value>
        public bool Granted { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the permission. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public PermissionResponseLinks Links { get; set; }
    }
}
