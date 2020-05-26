using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Only available for failed payments. Contains a failure reason code.
    /// </summary>
    public enum CreditCardFailureReason
    {
        /// <summary>
        /// The invalid card number
        /// </summary>
        [EnumMember(Value = "invalid_card_number")] InvalidCardNumber,
        /// <summary>
        /// The invalid CVV
        /// </summary>
        [EnumMember(Value = "invalid_cvv")] InvalidCvv,
        /// <summary>
        /// The invalid card holder name
        /// </summary>
        [EnumMember(Value = "invalid_card_holder_name")] InvalidCardHolderName,
        /// <summary>
        /// The card expired
        /// </summary>
        [EnumMember(Value = "card_expired")] CardExpired,
        /// <summary>
        /// The invalid card type
        /// </summary>
        [EnumMember(Value = "invalid_card_type")] InvalidCardType,
        /// <summary>
        /// The refused by issuer
        /// </summary>
        [EnumMember(Value = "refused_by_issuer")] RefusedByIssuer,
        /// <summary>
        /// The insufficient funds
        /// </summary>
        [EnumMember(Value = "insufficient_funds")] InsufficientFunds,
        /// <summary>
        /// The inactive card
        /// </summary>
        [EnumMember(Value = "inactive_card")] InactiveCard,
        /// <summary>
        /// The unknown reason
        /// </summary>
        [EnumMember(Value = "unknown_reason")] UnknownReason,
        /// <summary>
        /// The possible fraud
        /// </summary>
        [EnumMember(Value = "possible_fraud")] PossibleFraud
    }
}
