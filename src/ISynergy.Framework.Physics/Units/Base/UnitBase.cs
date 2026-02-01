using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Enumerations;
using ISynergy.Framework.Physics.Extensions;

namespace ISynergy.Framework.Physics.Base;

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
        Argument.IsNotNull(unit);

        Name = unit.GetDescription();
        Symbol = unit.GetSymbol();

        Argument.IsNotNullOrEmpty(Name);
        Argument.IsNotNullOrEmpty(Symbol);

        Argument.IsNotNull(unitTypes);
        Argument.Condition(unitTypes.Length, count => count >= 1, "At least one unit type must be specified");

        UnitTypes = unitTypes;
    }
}
