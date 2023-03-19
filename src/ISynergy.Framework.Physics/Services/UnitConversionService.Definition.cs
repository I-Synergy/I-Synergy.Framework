using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Enumerations;

namespace ISynergy.Framework.Physics.Services
{
    public partial class UnitConversionService
    {
        /// <summary>
        /// Add the SI and non-SI accepted units
        /// </summary>
        private void DefineSIUnits()
        {
            AddSIUnitDefinition(Enumerations.Units.second, new UnitTypes[] { UnitTypes.Time });
            AddSIUnitDefinition(Enumerations.Units.metre, new UnitTypes[] { UnitTypes.Length });
            AddSIUnitDefinition(Enumerations.Units.kilogram, new UnitTypes[] { UnitTypes.Mass });
            AddSIUnitDefinition(Enumerations.Units.ampere, new UnitTypes[] { UnitTypes.ElectricCurrent });
            AddSIUnitDefinition(Enumerations.Units.kelvin, new UnitTypes[] { UnitTypes.ThermodynamicTemperature });
            AddSIUnitDefinition(Enumerations.Units.mole, new UnitTypes[] { UnitTypes.AmountOfSubstance });
            AddSIUnitDefinition(Enumerations.Units.candela, new UnitTypes[] { UnitTypes.LuminousIntensity });

            AddSIUnitDefinition(Enumerations.Units.newton, new UnitTypes[] { UnitTypes.Force, UnitTypes.Weight });
            AddSIUnitDefinition(Enumerations.Units.joule, new UnitTypes[] { UnitTypes.Energy, UnitTypes.Work, UnitTypes.Heat });
            AddSIUnitDefinition(Enumerations.Units.hertz, new UnitTypes[] { UnitTypes.Frequency });
            AddSIUnitDefinition(Enumerations.Units.sqaure_metre, new UnitTypes[] { UnitTypes.Area });
            AddSIUnitDefinition(Enumerations.Units.cubic_metre, new UnitTypes[] { UnitTypes.Volume });
            AddSIUnitDefinition(Enumerations.Units.metre_per_second, new UnitTypes[] { UnitTypes.Speed, UnitTypes.Velocity });
            AddSIUnitDefinition(Enumerations.Units.metre_per_second_squared, new UnitTypes[] { UnitTypes.Acceleration });
            AddSIUnitDefinition(Enumerations.Units.kilogram_per_cubic_metre, new UnitTypes[] { UnitTypes.Density });
        }

        /// <summary>
        /// Add coherent derived units.
        /// </summary>
        private void DefineUnits()
        {
            // Time
            AddUnitDefinition(Enumerations.Units.minute, new UnitTypes[] { UnitTypes.Time }, (e) => e * 6e+1, (e) => e / 6e+1);
            AddUnitDefinition(Enumerations.Units.hour, new UnitTypes[] { UnitTypes.Time }, (e) => e * 36e+2, (e) => e / 36e+2);
            AddUnitDefinition(Enumerations.Units.day, new UnitTypes[] { UnitTypes.Time }, (e) => e * 8.64e+4, (e) => e / 8.64e+4);

            // Length
            AddUnitDefinition(Enumerations.Units.nanometre, new UnitTypes[] { UnitTypes.Length }, (e) => e / 1e+9, (e) => e * 1e+9);
            AddUnitDefinition(Enumerations.Units.micrometre, new UnitTypes[] { UnitTypes.Length }, (e) => e / 1e+6, (e) => e * 1e+6);
            AddUnitDefinition(Enumerations.Units.millimetre, new UnitTypes[] { UnitTypes.Length }, (e) => e / 1e+3, (e) => e * 1e+3);
            AddUnitDefinition(Enumerations.Units.centimetre, new UnitTypes[] { UnitTypes.Length }, (e) => e / 1e+2, (e) => e * 1e+2);
            AddUnitDefinition(Enumerations.Units.decimetre, new UnitTypes[] { UnitTypes.Length }, (e) => e / 1e+1, (e) => e * 1e+1);
            AddUnitDefinition(Enumerations.Units.inch, new UnitTypes[] { UnitTypes.Length }, (e) => e / 3.937e+1, (e) => e * 3.937e+1);
            AddUnitDefinition(Enumerations.Units.foot, new UnitTypes[] { UnitTypes.Length }, (e) => (e / 3.937e+1) * 12, (e) => (e / 12) * 3.937e+1);
            AddUnitDefinition(Enumerations.Units.yard, new UnitTypes[] { UnitTypes.Length }, (e) => (e / 3.937e+1) * 36, (e) => (e / 36) * 3.937e+1);
            AddUnitDefinition(Enumerations.Units.astronomical_unit, new UnitTypes[] { UnitTypes.Length }, (e) => e * 1.495978707e+11, (e) => e / 1.495978707e+11);

            // Force and Weight
            AddUnitDefinition(Enumerations.Units.kilogram_force, new UnitTypes[] { UnitTypes.Force, UnitTypes.Weight }, (e) => e * 9.80665e+0, (e) => e / 9.80665e+0);
            AddUnitDefinition(Enumerations.Units.gram_force, new UnitTypes[] { UnitTypes.Force, UnitTypes.Weight }, (e) => e * 9.80665e-3, (e) => e / 9.80665e-3);

            // Temperature
            AddUnitDefinition(Enumerations.Units.degree_Celcius, new UnitTypes[] { UnitTypes.ThermodynamicTemperature }, (e) => e + 273.15, (e) => e - 273.15);
            AddUnitDefinition(Enumerations.Units.degree_Farenheit, new UnitTypes[] { UnitTypes.ThermodynamicTemperature }, (e) => (5 * (e + 459.67)) / 9, (e) => e * 9 / 5 - 459.67);

            // Volume
            AddUnitDefinition(Enumerations.Units.litre, new UnitTypes[] { UnitTypes.Volume }, (e) => e * 10e-4, (e) => e / 10e-4);

            // Mass
            AddUnitDefinition(Enumerations.Units.tonne, new UnitTypes[] { UnitTypes.Mass }, (e) => e * 1e+3, (e) => e / 1e+3);

            // Area
            AddUnitDefinition(Enumerations.Units.hectare, new UnitTypes[] { UnitTypes.Area }, (e) => e * 10e+4, (e) => e / 10e+4);
        }

        /// <summary>
        /// A list of all units (SI units included) which can we queried.
        /// </summary>
        public List<IUnit> Units { get; } = new List<IUnit>();

        private void AddSIUnitDefinition(Enumerations.Units unit, UnitTypes[] unitTypes) =>
            Units.Add(new SIUnit(unit, unitTypes));

        private void AddUnitDefinition(Enumerations.Units unit, UnitTypes[] unitTypes, Func<double, double> formulaConvert, Func<double, double> formulaConvertBack) =>
            Units.Add(new Unit(unit, unitTypes, formulaConvert, formulaConvertBack));
    }
}
