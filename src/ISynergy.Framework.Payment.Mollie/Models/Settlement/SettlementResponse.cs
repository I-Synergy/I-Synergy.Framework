using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Settlement
{
    /// <summary>
    /// Class SettlementResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class SettlementResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a settlement object. Will always contain settlement for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The settlement's identifier, for example stl_jDk30akdN.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The settlement's bank reference, as found on your invoice and in your ISynergy.Framework.Payment.Mollie account.
        /// </summary>
        /// <value>The reference.</value>
        public string Reference { get; set; }

        /// <summary>
        /// The date on which the settlement was created.
        /// When requesting the next settlement the returned date signifies the expected settlement date.
        /// When requesting the open settlement (open funds) the return value is null.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The date on which the settlement was settled.
        /// When requesting the open settlement or next settlement the return value is null.
        /// </summary>
        /// <value>The settled at.</value>
        public DateTime? SettledAt { get; set; }

        /// <summary>
        /// The status of the settlement.
        /// </summary>
        /// <value>The status.</value>
        public SettlementStatus Status { get; set; }

        /// <summary>
        /// The total amount paid out with this settlement.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// This object is a collection of Period objects, which describe the settlement by month in full detail.
        /// Please refer to the Period object section below.
        /// </summary>
        /// <value>The periods.</value>
        public Dictionary<int, Dictionary<int, SettlementPeriod>> Periods { get; set; }

        /// <summary>
        /// A list of all payment IDs that make up the settlement. You can use this to fully reconciliate the settlement with your back office.
        /// </summary>
        /// <value>The payment ids.</value>
        public List<string> PaymentIds { get; set; }

        /// <summary>
        /// A list of all refund IDs that make up the settlement. You can use this to fully reconciliate the settlement with your back office.
        /// </summary>
        /// <value>The refund ids.</value>
        public List<string> RefundIds { get; set; }

        /// <summary>
        /// A list of all chargeback IDs that make up the settlement. You can use this to fully reconciliate the settlement with your back office.
        /// </summary>
        /// <value>The chargeback ids.</value>
        public List<string> ChargebackIds { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the settlement. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public SettlementResponseLinks Links { get; set; }
    }
}
