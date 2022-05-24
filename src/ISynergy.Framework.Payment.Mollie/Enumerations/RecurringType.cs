using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum RecurringType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecurringType
    {
        /// <summary>
        /// The first
        /// </summary>
        [EnumMember(Value = "first")]
        First,
        /// <summary>
        /// The recurring
        /// </summary>
        [EnumMember(Value = "recurring")]
        Recurring
    }
}
