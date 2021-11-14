namespace ISynergy.Framework.Physics.Base
{
    /// <summary>
    /// Base Unit
    /// </summary>
    public abstract class UnitBase : IUnit
    {
        /// <summary>
        /// Name of the unit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Symbol of the unit.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Unit types for this unit.
        /// </summary>
        public UnitTypes[] UnitTypes { get; }

        /// <summary>
        /// Default constructor for creating a base unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="unitTypes"></param>
        protected UnitBase(Units unit, UnitTypes[] unitTypes)
        {
            Argument.IsNotNull(nameof(unit), unit);

            Name = unit.GetDescription();
            Symbol = unit.GetSymbol();

            Argument.IsNotNullOrEmpty(nameof(Name), Name);
            Argument.IsNotNullOrEmpty(nameof(Symbol), Symbol);

            Argument.IsNotNull(nameof(unitTypes), unitTypes);
            Argument.Condition(nameof(unitTypes), unitTypes, value => unitTypes.Count() >= 1);

            UnitTypes = unitTypes;
        }
    }
}
