using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// The mode used to create this payment. Mode determines whether a payment is real or a test payment.
    /// </summary>
    public enum PaymentMode
    {
        /// <summary>
        /// The live
        /// </summary>
        [EnumMember(Value = "live")]
        Live,
        /// <summary>
        /// The test
        /// </summary>
        [EnumMember(Value = "test")]
        Test
    }
}
