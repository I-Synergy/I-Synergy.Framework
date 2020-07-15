using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum OrderStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        /// <summary>
        /// The created
        /// </summary>
        [EnumMember(Value = "created")] Created,
        /// <summary>
        /// The paid
        /// </summary>
        [EnumMember(Value = "paid")] Paid,
        /// <summary>
        /// The authorized
        /// </summary>
        [EnumMember(Value = "authorized")] Authorized,
        /// <summary>
        /// The canceled
        /// </summary>
        [EnumMember(Value = "canceled")] Canceled,
        /// <summary>
        /// The shipping
        /// </summary>
        [EnumMember(Value = "shipping")] Shipping,
        /// <summary>
        /// The completed
        /// </summary>
        [EnumMember(Value = "completed")] Completed,
        /// <summary>
        /// The expired
        /// </summary>
        [EnumMember(Value = "expired")] Expired
    }
}
