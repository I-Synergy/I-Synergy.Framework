using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum OrderLineStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderLineStatus
    {
        /// <summary>
        /// The created
        /// </summary>
        [EnumMember(Value = "created")] Created,
        /// <summary>
        /// The paid
        /// </summary>
        [EnumMember(Value = "paid")] Paid,
        /// <summary>
        /// The authorized
        /// </summary>
        [EnumMember(Value = "authorized")] Authorized,
        /// <summary>
        /// The canceled
        /// </summary>
        [EnumMember(Value = "canceled")] Canceled,
        /// <summary>
        /// The shipping
        /// </summary>
        [EnumMember(Value = "shipping")] Shipping,
        /// <summary>
        /// The completed
        /// </summary>
        [EnumMember(Value = "completed")] Completed,
    }
}
