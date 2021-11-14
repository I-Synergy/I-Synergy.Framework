namespace ISynergy.Framework.Physics
{
    /// <summary>
    /// Base SI Unit.
    /// The SI base units form a set of mutually independent dimensions as required by dimensional analysis commonly employed in science and technology.
    /// The names and symbols of SI base units are written in lowercase, except the symbols of those named after a person, which are written with an initial capital letter. 
    /// For example, the metre (US English: meter) has the symbol m, but the kelvin has symbol K, because it is named after Lord Kelvin and the ampere with symbol A is named after André-Marie Ampère.
    /// A number of other units, such as the litre(US English: liter), astronomical unit and electronvolt, are not formally part of the SI, but are accepted for use with SI.
    /// </summary>
    public class SIUnit : UnitBase
    {
        /// <summary>
        /// Default constructor for creating a SI base unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="unitTypes"></param>
        public SIUnit(Units unit, UnitTypes[] unitTypes)
            : base(unit, unitTypes)
        {
        }
    }
}
