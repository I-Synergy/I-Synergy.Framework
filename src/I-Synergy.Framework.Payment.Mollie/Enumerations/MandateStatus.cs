using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum MandateStatus
    /// </summary>
    public enum MandateStatus
    {
        /// <summary>
        /// The valid
        /// </summary>
        [EnumMember(Value = "valid")]
        Valid,
        /// <summary>
        /// The invalid
        /// </summary>
        [EnumMember(Value = "invalid")]
        Invalid,
        /// <summary>
        /// The pending
        /// </summary>
        [EnumMember(Value = "pending")]
        Pending
    }
}
