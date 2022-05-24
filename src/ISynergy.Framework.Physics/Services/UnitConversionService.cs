using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Enumerations;
using ISynergy.Framework.Physics.Extensions;
using System;
using System.Linq;

namespace ISynergy.Framework.Physics.Services
{
    /// <summary>
    /// Service for handling matters that concern units and SI units.
    /// </summary>
    public partial class UnitConversionService : IUnitConversionService
    {
        /// <summary>
        /// Private service for accessing resource files for localization.
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Default constructor of the Unit Service.
        /// </summary>
        public UnitConversionService(ILanguageService languageService)
        {
            _languageService = languageService;

            DefineSIUnits();
            DefineUnits();
        }

        /// <summary>
        /// Converter to convert one Unit to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns>double</returns>
        /// <remarks>
        /// Conversion can only be done for units who have the same unit type.
        /// if neither source or target are SI units, conversion will be done via an intermediate SI Unit.
        /// </remarks>
        public double Convert(IUnit source, double value, IUnit target)
        {
            Argument.IsNotNull(source);
            Argument.IsNotNull(value);
            Argument.IsNotNull(target);

            if (target.Equals(source))
                return value;

            if (source is SIUnit && target is SIUnit)
                throw new ArgumentException("For conversion the source and target cannot be both an SI unit. e.g. metre and metre ");

            if (!target.UnitTypes.Intersect(source.UnitTypes).Any())
                throw new ArgumentException("For conversion the source and target must have the same UnitType. e.g. Time and Length");

            // source is SI Unit
            if (source is SIUnit && target is Unit t)
                return t.FormulaConvertBack(value);

            // target is SI Unit
            if (source is Unit s && target is SIUnit)
                return s.FormulaConvert(value);

            // if both units are non-SI units and have an intersecting SI unit
            else if (source is Unit s2 && target is Unit t2 && t2.UnitTypes.Intersect(s2.UnitTypes).Any())
            {
                //Get SI unit of source
                var sourceSI = Units
                    .Where(q => q.UnitTypes.Intersect(s2.UnitTypes).Any() && q is SIUnit)
                    .Single();

                //Get SI unit of target
                var targetSI = Units
                    .Where(q => q.UnitTypes.Intersect(t2.UnitTypes).Any() && q is SIUnit)
                    .Single();

                return t2.FormulaConvertBack(s2.FormulaConvert(value));
            }

            throw new Exception("Conversion exception occured.");
        }

        /// <summary>
        /// Converter to convert from symbol to another symbol.
        /// </summary>
        /// <param name="sourceSymbol"></param>
        /// <param name="value"></param>
        /// <param name="targetSymbol"></param>
        /// <returns></returns>
        public double Convert(string sourceSymbol, double value, string targetSymbol)
        {
            Argument.IsNotNull(sourceSymbol);
            Argument.IsNotNull(value);
            Argument.IsNotNull(targetSymbol);

            if (targetSymbol.Equals(sourceSymbol))
                return value;

            if (Units.Where(q => q.Symbol.Equals(sourceSymbol)).Single() is IUnit source &&
                Units.Where(q => q.Symbol.Equals(targetSymbol)).Single() is IUnit target)
                return Convert(source, value, target);

            throw new ArgumentException("Converter failed to get the corresponding units.");
        }

        /// <summary>
        /// Converter to convert from unit enumeration to another unit enumeration.
        /// </summary>
        /// <param name="sourceUnit"></param>
        /// <param name="value"></param>
        /// <param name="targetUnit"></param>
        /// <returns></returns>
        public double Convert(Units sourceUnit, double value, Units targetUnit)
        {
            Argument.IsNotNull(sourceUnit);
            Argument.IsNotNull(value);
            Argument.IsNotNull(targetUnit);

            if (targetUnit.Equals(sourceUnit))
                return value;

            if (Units.Where(q => q.Symbol.Equals(sourceUnit.GetSymbol())).Single() is IUnit source &&
                Units.Where(q => q.Symbol.Equals(targetUnit.GetSymbol())).Single() is IUnit target)
                return Convert(source, value, target);

            throw new ArgumentException("Converter failed to get the corresponding units.");
        }
    }
}
