using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum KbcIssuer
    /// </summary>
    public enum KbcIssuer
    {
        /// <summary>
        /// The KBC
        /// </summary>
        [EnumMember(Value = "kbc")]
        Kbc,
        /// <summary>
        /// The CBC
        /// </summary>
        [EnumMember(Value = "cbc")]
        Cbc
    }
}
