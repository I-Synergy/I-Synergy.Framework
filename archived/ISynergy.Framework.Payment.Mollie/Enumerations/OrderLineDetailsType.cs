using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum OrderLineDetailsType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderLineDetailsType
    {
        /// <summary>
        /// The physical
        /// </summary>
        [EnumMember(Value = "physical")] Physical,
        /// <summary>
        /// The discount
        /// </summary>
        [EnumMember(Value = "discount")] Discount,
        /// <summary>
        /// The digital
        /// </summary>
        [EnumMember(Value = "digital")] Digital,
        /// <summary>
        /// The shipping fee
        /// </summary>
        [EnumMember(Value = "shipping_fee")] ShippingFee,
        /// <summary>
        /// The store credit
        /// </summary>
        [EnumMember(Value = "store_credit")] StoreCredit,
        /// <summary>
        /// The gift card
        /// </summary>
        [EnumMember(Value = "gift_card")] GiftCard,
        /// <summary>
        /// The surcharge
        /// </summary>
        [EnumMember(Value = "surcharge")] Surcharge,
    }
}
