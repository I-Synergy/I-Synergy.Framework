namespace ISynergy.Common.Calculations
{
    public class Metric
    {
        /// <summary>
        /// Berekend het oppervlakte
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <returns>Decimal</returns>
        /// <remarks></remarks>
        public static decimal Surface(decimal length, decimal width)
        {
            return (length * width);
        }

        /// <summary>
        /// Berekend het volume
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Decimal</returns>
        /// <remarks></remarks>
        public static decimal Volume(decimal length, decimal width, decimal height)
        {
            return (length * width * height);
        }

        /// <summary>
        /// Berekend de Dichtheid
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="weight"></param>
        /// <returns>Decimal</returns>
        /// <remarks></remarks>
        public static decimal Density(decimal volume, decimal weight)
        {
            return (weight / volume);
        }
    }
}