using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Only available if the payment has been completed – The type of security used during payment processing.
    /// </summary>
    public enum CreditCardSecurity
    {
        /// <summary>
        /// The normal
        /// </summary>
        Normal,
        /// <summary>
        /// The secure3 d
        /// </summary>
        [EnumMember(Value = "3dsecure")]
        Secure3D
    }
}
