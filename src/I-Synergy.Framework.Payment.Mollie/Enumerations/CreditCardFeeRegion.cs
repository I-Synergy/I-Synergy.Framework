using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Only available if the payment has been completed – The fee region for the payment: intra-eu for consumer cards from the EU, and
    /// other for all other cards.
    /// </summary>
    public enum CreditCardFeeRegion
    {
        /// <summary>
        /// The intra eu
        /// </summary>
        [EnumMember(Value = "intra-eu")] IntraEu,
        /// <summary>
        /// The other
        /// </summary>
        [EnumMember(Value = "other")] Other,
        /// <summary>
        /// The american express
        /// </summary>
        [EnumMember(Value = "american-express")] AmericanExpress,
        /// <summary>
        /// The carte bancaire
        /// </summary>
        [EnumMember(Value = "carte-bancaire")] CarteBancaire,
        /// <summary>
        /// The maestro
        /// </summary>
        [EnumMember(Value = "maestro")] Maestro,
    }
}
