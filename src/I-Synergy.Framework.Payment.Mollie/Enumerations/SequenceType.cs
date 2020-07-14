using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum SequenceType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SequenceType
    {
        /// <summary>
        /// The one off
        /// </summary>
        [EnumMember(Value = "oneoff")] OneOff,
        /// <summary>
        /// The first
        /// </summary>
        [EnumMember(Value = "first")] First,
        /// <summary>
        /// The recurring
        /// </summary>
        [EnumMember(Value = "recurring")] Recurring
    }
}
