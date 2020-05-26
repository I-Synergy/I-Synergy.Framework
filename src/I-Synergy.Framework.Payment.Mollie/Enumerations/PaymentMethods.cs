using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum PaymentMethods
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethods
    {
        /// <summary>
        /// The bancontact
        /// </summary>
        [EnumMember(Value = "bancontact")] Bancontact,
        /// <summary>
        /// The bank transfer
        /// </summary>
        [EnumMember(Value = "banktransfer")] BankTransfer,
        /// <summary>
        /// The belfius
        /// </summary>
        [EnumMember(Value = "belfius")] Belfius,
        /// <summary>
        /// The bitcoin
        /// </summary>
        [EnumMember(Value = "bitcoin")] Bitcoin,
        /// <summary>
        /// The credit card
        /// </summary>
        [EnumMember(Value = "creditcard")] CreditCard,
        /// <summary>
        /// The direct debit
        /// </summary>
        [EnumMember(Value = "directdebit")] DirectDebit,
        /// <summary>
        /// The eps
        /// </summary>
        [EnumMember(Value = "eps")] Eps,
        /// <summary>
        /// The gift card
        /// </summary>
        [EnumMember(Value = "giftcard")] GiftCard,
        /// <summary>
        /// The giropay
        /// </summary>
        [EnumMember(Value = "giropay")] Giropay,
        /// <summary>
        /// The ideal
        /// </summary>
        [EnumMember(Value = "ideal")] Ideal,
        /// <summary>
        /// The ing home pay
        /// </summary>
        [EnumMember(Value = "inghomepay")] IngHomePay,
        /// <summary>
        /// The KBC
        /// </summary>
        [EnumMember(Value = "kbc")] Kbc,
        /// <summary>
        /// The pay pal
        /// </summary>
        [EnumMember(Value = "paypal")] PayPal,
        /// <summary>
        /// The pay safe card
        /// </summary>
        [EnumMember(Value = "paysafecard")] PaySafeCard,
        /// <summary>
        /// The sofort
        /// </summary>
        [EnumMember(Value = "sofort")] Sofort,
        /// <summary>
        /// The refund
        /// </summary>
        [EnumMember(Value = "refund")] Refund,
        /// <summary>
        /// The klarna pay later
        /// </summary>
        [EnumMember(Value = "klarnapaylater")] KlarnaPayLater,
        /// <summary>
        /// The klarna slice it
        /// </summary>
        [EnumMember(Value = "klarnasliceit")] KlarnaSliceIt,
        /// <summary>
        /// The przelewy24
        /// </summary>
        [EnumMember(Value = "przelewy24")] Przelewy24,
        /// <summary>
        /// The apple pay
        /// </summary>
        [EnumMember(Value = "applepay")] ApplePay,
    }
}
