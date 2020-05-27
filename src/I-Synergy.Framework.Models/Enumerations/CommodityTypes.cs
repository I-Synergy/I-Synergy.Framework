using System;

namespace ISynergy.Models.Enumerations
{
    /// <summary>
    /// Enum CommodityTypes
    /// </summary>
    [Flags]
    public enum CommodityTypes
    {
        /// <summary>
        /// The sales
        /// </summary>
        Sales = 0,
        /// <summary>
        /// The service
        /// </summary>
        Service = 1,
        /// <summary>
        /// The offer
        /// </summary>
        Offer = 2,
        /// <summary>
        /// The consumable
        /// </summary>
        Consumable = 4,
        /// <summary>
        /// The part
        /// </summary>
        Part = 8,
        /// <summary>
        /// The semi finished
        /// </summary>
        SemiFinished = 16,
        /// <summary>
        /// The session
        /// </summary>
        Session = 32
    }
}
