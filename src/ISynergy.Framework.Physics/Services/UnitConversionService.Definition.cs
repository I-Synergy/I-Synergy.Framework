using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Enumerations;

namespace ISynergy.Framework.Physics.Services;

public partial class UnitConversionService
{
    /// <summary>
    /// Add the SI and non-SI accepted units
    /// </summary>
    private void DefineSIUnits()
    {
        AddSIUnitDefinition(Enumerations.Units.second, [UnitTypes.Time]);
        AddSIUnitDefinition(Enumerations.Units.metre, [UnitTypes.Length]);
        AddSIUnitDefinition(Enumerations.Units.kilogram, [UnitTypes.Mass]);
        AddSIUnitDefinition(Enumerations.Units.ampere, [UnitTypes.ElectricCurrent]);
        AddSIUnitDefinition(Enumerations.Units.kelvin, [UnitTypes.ThermodynamicTemperature]);
        AddSIUnitDefinition(Enumerations.Units.mole, [UnitTypes.AmountOfSubstance]);
        AddSIUnitDefinition(Enumerations.Units.candela, [UnitTypes.LuminousIntensity]);

        AddSIUnitDefinition(Enumerations.Units.newton, [UnitTypes.Force, UnitTypes.Weight]);
        AddSIUnitDefinition(Enumerations.Units.joule, [UnitTypes.Energy, UnitTypes.Work, UnitTypes.Heat]);
        AddSIUnitDefinition(Enumerations.Units.hertz, [UnitTypes.Frequency]);
        AddSIUnitDefinition(Enumerations.Units.sqaure_metre, [UnitTypes.Area]);
        AddSIUnitDefinition(Enumerations.Units.cubic_metre, [UnitTypes.Volume]);
        AddSIUnitDefinition(Enumerations.Units.metre_per_second, [UnitTypes.Speed, UnitTypes.Velocity]);
        AddSIUnitDefinition(Enumerations.Units.metre_per_second_squared, [UnitTypes.Acceleration]);
        AddSIUnitDefinition(Enumerations.Units.kilogram_per_cubic_metre, [UnitTypes.Density]);
    }

    /// <summary>
    /// Add coherent derived units.
    /// </summary>
    private void DefineUnits()
    {
        // Time
        AddUnitDefinition(Enumerations.Units.minute, [UnitTypes.Time], (e) => e * 6e+1, (e) => e / 6e+1);
        AddUnitDefinition(Enumerations.Units.hour, [UnitTypes.Time], (e) => e * 36e+2, (e) => e / 36e+2);
        AddUnitDefinition(Enumerations.Units.day, [UnitTypes.Time], (e) => e * 8.64e+4, (e) => e / 8.64e+4);

        // Length
        AddUnitDefinition(Enumerations.Units.nanometre, [UnitTypes.Length], (e) => e / 1e+9, (e) => e * 1e+9);
        AddUnitDefinition(Enumerations.Units.micrometre, [UnitTypes.Length], (e) => e / 1e+6, (e) => e * 1e+6);
        AddUnitDefinition(Enumerations.Units.millimetre, [UnitTypes.Length], (e) => e / 1e+3, (e) => e * 1e+3);
        AddUnitDefinition(Enumerations.Units.centimetre, [UnitTypes.Length], (e) => e / 1e+2, (e) => e * 1e+2);
        AddUnitDefinition(Enumerations.Units.decimetre, [UnitTypes.Length], (e) => e / 1e+1, (e) => e * 1e+1);
        AddUnitDefinition(Enumerations.Units.inch, [UnitTypes.Length], (e) => e / 3.937e+1, (e) => e * 3.937e+1);
        AddUnitDefinition(Enumerations.Units.foot, [UnitTypes.Length], (e) => (e / 3.937e+1) * 12, (e) => (e / 12) * 3.937e+1);
        AddUnitDefinition(Enumerations.Units.yard, [UnitTypes.Length], (e) => (e / 3.937e+1) * 36, (e) => (e / 36) * 3.937e+1);
        AddUnitDefinition(Enumerations.Units.astronomical_unit, [UnitTypes.Length], (e) => e * 1.495978707e+11, (e) => e / 1.495978707e+11);

        // Force and Weight
        AddUnitDefinition(Enumerations.Units.kilogram_force, [UnitTypes.Force, UnitTypes.Weight], (e) => e * 9.80665e+0, (e) => e / 9.80665e+0);
        AddUnitDefinition(Enumerations.Units.gram_force, [UnitTypes.Force, UnitTypes.Weight], (e) => e * 9.80665e-3, (e) => e / 9.80665e-3);

        // Temperature
        AddUnitDefinition(Enumerations.Units.degree_Celcius, [UnitTypes.ThermodynamicTemperature], (e) => e + 273.15, (e) => e - 273.15);
        AddUnitDefinition(Enumerations.Units.degree_Farenheit, [UnitTypes.ThermodynamicTemperature], (e) => (5 * (e + 459.67)) / 9, (e) => e * 9 / 5 - 459.67);

        // Volume
        AddUnitDefinition(Enumerations.Units.litre, [UnitTypes.Volume], (e) => e * 10e-4, (e) => e / 10e-4);

        // Mass
        AddUnitDefinition(Enumerations.Units.tonne, [UnitTypes.Mass], (e) => e * 1e+3, (e) => e / 1e+3);

        // Area
        AddUnitDefinition(Enumerations.Units.hectare, [UnitTypes.Area], (e) => e * 10e+4, (e) => e / 10e+4);
    }

    /// <summary>
    /// A list of all units (SI units included) which can we queried.
    /// </summary>
    public List<IUnit> Units { get; } = [];

    private void AddSIUnitDefinition(Enumerations.Units unit, UnitTypes[] unitTypes) =>
        Units.Add(new SIUnit(unit, unitTypes));

    private void AddUnitDefinition(Enumerations.Units unit, UnitTypes[] unitTypes, Func<double, double> formulaConvert, Func<double, double> formulaConvertBack) =>
        Units.Add(new Unit(unit, unitTypes, formulaConvert, formulaConvertBack));
}
