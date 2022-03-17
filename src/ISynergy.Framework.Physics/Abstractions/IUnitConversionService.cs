using ISynergy.Framework.Physics.Enumerations;
using System.Collections.Generic;

namespace ISynergy.Framework.Physics.Abstractions
{
    /// <summary>
    /// Service for handling matters that concern units and SI units.
    /// </summary>
    public interface IUnitConversionService
    {
        /// <summary>
        /// A list of all units (SI units included) which can we queried.
        /// </summary>
        List<IUnit> Units { get; }

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
        double Convert(IUnit source, double value, IUnit target);

        /// <summary>
        /// Converter to convert from symbol to another symbol.
        /// </summary>
        /// <param name="sourceSymbol"></param>
        /// <param name="value"></param>
        /// <param name="targetSymbol"></param>
        /// <returns></returns>
        double Convert(string sourceSymbol, double value, string targetSymbol);

        /// <summary>
        /// Converter to convert from unit enumeration to another unit enumeration.
        /// </summary>
        /// <param name="sourceUnit"></param>
        /// <param name="value"></param>
        /// <param name="targetUnit"></param>
        /// <returns></returns>
        double Convert(Units sourceUnit, double value, Units targetUnit);
    }
}