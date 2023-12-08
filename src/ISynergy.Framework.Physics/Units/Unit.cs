using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Physics.Base;
using ISynergy.Framework.Physics.Enumerations;

namespace ISynergy.Framework.Physics;

/// <summary>
/// Unit derived from a SI unit.
/// </summary>
public class Unit : UnitBase
{
    /// <summary>
    /// Formula to convert unit to SI unit.
    /// </summary>
    public Func<double, double> FormulaConvert;

    /// <summary>
    /// Formula to convert back from SI unit to this unit.
    /// </summary>
    public Func<double, double> FormulaConvertBack;

    /// <summary>
    /// Default contructor for Units
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="unitTypes"></param>
    /// <param name="formulaConvert">Formula to convert to SI unit.</param>
    /// <param name="formulaConvertBack">Formula to convert back from SI unit to this unit.</param>
    public Unit(Units unit, UnitTypes[] unitTypes, Func<double, double> formulaConvert, Func<double, double> formulaConvertBack)
        : base(unit, unitTypes)
    {
        Argument.IsNotNull(formulaConvert);
        Argument.IsNotNull(formulaConvertBack);

        FormulaConvert = formulaConvert;
        FormulaConvertBack = formulaConvertBack;
    }
}
