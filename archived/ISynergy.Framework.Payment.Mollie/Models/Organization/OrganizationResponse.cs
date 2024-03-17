using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Organization
{
    /// <summary>
    /// Class OrganizationResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class OrganizationResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a organization object.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The organization's identifier, for example org_1234567.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The organization's official name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// The address of the organization.
        /// </summary>
        /// <value>The address.</value>
        public AddressObject Address { get; set; }

        /// <summary>
        /// The registration number of the organization at the (local) chamber of commerce.
        /// </summary>
        /// <value>The registration number.</value>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// The VAT number of the organization, if based in the European Union. The VAT number has been checked with the VIES by ISynergy.Framework.Payment.Mollie.
        /// </summary>
        /// <value>The vat number.</value>
        public string VatNumber { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the organization. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public OrganizationResponseLinks Links { get; set; }
    }
}
