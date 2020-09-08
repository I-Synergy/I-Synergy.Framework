using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// The card's label. Note that not all labels can be acquired through ISynergy.Framework.Payment.Mollie.
    /// </summary>
    public enum CreditCardLabel
    {
        /// <summary>
        /// The american express
        /// </summary>
        [EnumMember(Value = "American Express")] AmericanExpress,
        /// <summary>
        /// The carta si
        /// </summary>
        [EnumMember(Value = "Carta si")] CartaSi,
        /// <summary>
        /// The carte bleue
        /// </summary>
        [EnumMember(Value = "Carte Bleue")] CarteBleue,
        /// <summary>
        /// The dankort
        /// </summary>
        Dankort,
        /// <summary>
        /// The diners club
        /// </summary>
        [EnumMember(Value = "Diners Club")] DinersClub,
        /// <summary>
        /// The discover
        /// </summary>
        Discover,
        /// <summary>
        /// The JCB
        /// </summary>
        [EnumMember(Value = "JCB")] Jcb,
        /// <summary>
        /// The laser
        /// </summary>
        [EnumMember(Value = "Laser")] Laser,
        /// <summary>
        /// The maestro
        /// </summary>
        Maestro,
        /// <summary>
        /// The mastercard
        /// </summary>
        Mastercard,
        /// <summary>
        /// The unionpay
        /// </summary>
        Unionpay,
        /// <summary>
        /// The visa
        /// </summary>
        Visa
    }
}
