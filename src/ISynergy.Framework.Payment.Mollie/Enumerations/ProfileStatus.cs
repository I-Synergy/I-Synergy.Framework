using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum ProfileStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProfileStatus
    {
        /// <summary>
        /// The unverified
        /// </summary>
        [EnumMember(Value = "unverified")]
        Unverified,
        /// <summary>
        /// The verified
        /// </summary>
        [EnumMember(Value = "verified")]
        Verified,
        /// <summary>
        /// The blocked
        /// </summary>
        [EnumMember(Value = "blocked")]
        Blocked
    }
}
