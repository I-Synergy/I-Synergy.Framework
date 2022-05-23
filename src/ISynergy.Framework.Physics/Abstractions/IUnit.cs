using ISynergy.Framework.Physics.Enumerations;

namespace ISynergy.Framework.Physics.Abstractions
{
    /// <summary>
    /// public interface for units and SI units.
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// Name of the unit.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Symbol of the unit.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Unit types for this unit.
        /// </summary>
        UnitTypes[] UnitTypes { get; }
    }
}
