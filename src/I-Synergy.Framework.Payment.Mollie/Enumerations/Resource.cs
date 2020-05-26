using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum Resource
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Resource
    {
        /// <summary>
        /// The orders
        /// </summary>
        [EnumMember(Value = "orders")] Orders,
        /// <summary>
        /// The payments
        /// </summary>
        [EnumMember(Value = "payments")] Payments
    }
}
