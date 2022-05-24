using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.Physics.Enumerations
{
    /// <summary>
    /// The SI base units are the standard units of measurement defined by the International System of Units (SI) for the seven base quantities of what is now known as the International System of Quantities.
    /// They are notably a basic set from which all other SI units can be derived. 
    /// </summary>
    public enum UnitTypes
    {
        /// <summary>
        /// The second, symbol s, is the SI unit of time.
        /// It is defined by taking the fixed numerical value of the caesium frequency ∆νCs, the unperturbed ground-state hyperfine transition frequency of the caesium 133 atom, to be 9192631770 when expressed in the unit Hz, which is equal to s−1.
        /// </summary>
        [LocalizedDescription("Time")]
        [Symbol("T")]
        Time,
        /// <summary>
        /// The metre, symbol m, is the SI unit of length.
        /// It is defined by taking the fixed numerical value of the speed of light in vacuum c to be 299792458 when expressed in the unit m s−1, where the second is defined in terms of ∆νCs.
        /// </summary>
        [LocalizedDescription("Length")]
        [Symbol("L")]
        Length,
        /// <summary>
        /// The kilogram, symbol kg, is the SI unit of mass.
        /// It is defined by taking the fixed numerical value of the Planck constant h to be 6.62607015×10−34 when expressed in the unit J s, which is equal to kg m2 s−1, where the metre and the second are defined in terms of c and ∆νCs.
        /// </summary>
        [LocalizedDescription("Mass")]
        [Symbol("M")]
        Mass,
        /// <summary>
        /// The ampere, symbol A, is the SI unit of electric current.
        /// It is defined by taking the fixed numerical value of the elementary charge e to be 1.602176634×10−19 when expressed in the unit C, which is equal to A s, where the second is defined in terms of ∆νCs.
        /// </summary>
        [LocalizedDescription("ElectricCurrent")]
        [Symbol("I")]
        ElectricCurrent,
        /// <summary>
        /// The kelvin, symbol K, is the SI unit of thermodynamic temperature.
        /// It is defined by taking the fixed numerical value of the Boltzmann constant k to be 1.380649×10−23 when expressed in the unit J K−1, which is equal to kg m2 s−2 K−1, where the kilogram, metre and second are defined in terms of h, c and ∆νCs.
        /// </summary>
        [LocalizedDescription("ThermodynamicTemperature")]
        [Symbol("Θ")]
        ThermodynamicTemperature,
        /// <summary>
        /// The mole, symbol mol, is the SI unit of amount of substance.
        /// One mole contains exactly 6.022 140 76 × 1023 elementary entities. This number is the fixed numerical value of the Avogadro constant, NA, when expressed in the unit mol−1 and is called the Avogadro number. The amount of substance, symbol n, of a system is a measure of the number of specified elementary entities. An elementary entity may be an atom, a molecule, an ion, an electron, any other particle or specified group of particles.
        /// </summary>
        [LocalizedDescription("AmountOfSubstance")]
        [Symbol("N")]
        AmountOfSubstance,
        /// <summary>
        /// The candela, symbol cd, is the SI unit of luminous intensity in a given direction.
        /// It is defined by taking the fixed numerical value of the luminous efficacy of monochromatic radiation of frequency 540×1012 Hz, Kcd, to be 683 when expressed in the unit lm W−1, which is equal to cd sr W−1, or cd sr kg−1 m−2 s3, where the kilogram, metre and second are defined in terms of h, c and ∆νCs.
        /// </summary>
        [LocalizedDescription("LuminousIntensity")]
        [Symbol("J")]
        LuminousIntensity,
        /// <summary>
        /// Volume is the quantity of three-dimensional space enclosed by a closed surface, for example, the space that a substance (solid, liquid, gas, or plasma) or 3D shape occupies or contains.
        /// Volume is often quantified numerically using the SI derived unit, the cubic metre. The volume of a container is generally understood to be the capacity of the container; 
        /// </summary>
        [LocalizedDescription("Volume")]
        [Symbol("V")]
        Volume,
        /// <summary>
        /// Energy is the quantitative property that must be transferred to a body or physical system to perform work on the body, or to heat it. 
        /// Energy is a conserved quantity; the law of conservation of energy states that energy can be converted in form, but not created or destroyed. 
        /// The unit of measurement in the International System of Units (SI) of energy is the joule, which is the energy transferred to an object by the work of moving it a distance of one metre against a force of one newton.
        /// </summary>
        [LocalizedDescription("Energy")]
        [Symbol("E")]
        Energy,
        /// <summary>
        /// The radian, denoted by the symbol rad is the SI unit for measuring angles, and is the standard unit of angular measure used in many areas of mathematics.
        /// The unit was formerly an SI supplementary unit (before that category was abolished in 1995) and the radian is now an SI derived unit.
        /// The radian is defined in the SI as being a dimensionless value, and its symbol is accordingly often omitted, especially in mathematical writing.
        /// </summary>
        [LocalizedDescription("AnglePlane")]
        AnglePlane,
        /// <summary>
        /// The steradian (symbol: sr) or square radian is the SI unit of solid angle.
        /// It is used in three-dimensional geometry, and is analogous to the radian, which quantifies planar angles. Whereas an angle in radians, projected onto a circle, gives a length on the circumference, a solid angle in steradians, projected onto a sphere, gives an area on the surface.
        /// The name is derived from the Greek στερεός stereos 'solid' + radian.
        /// The steradian, like the radian, is a dimensionless unit, the quotient of the area subtended and the square of its distance from the center. 
        /// Both the numerator and denominator of this ratio have dimension length squared (i.e. L2/L2 = 1, dimensionless). 
        /// It is useful, however, to distinguish between dimensionless quantities of a different nature, so the symbol "sr" is used to indicate a solid angle. 
        /// For example, radiant intensity can be measured in watts per steradian (W⋅sr−1). The steradian was formerly an SI supplementary unit, but this category was abolished in 1995 and the steradian is now considered an SI derived unit.
        /// </summary>
        [LocalizedDescription("AngleSolid")]
        [Symbol("Ω")]
        AngleSolid,
        /// <summary>
        /// Force is any interaction that, when unopposed, will change the motion of an object. 
        /// A force can cause an object with mass to change its velocity (which includes to begin moving from a state of rest), i.e., to accelerate. 
        /// Force can also be described intuitively as a push or a pull.
        /// A force has both magnitude and direction, making it a vector quantity. It is measured in the SI unit of newton (N). Force is represented by the symbol F.
        /// </summary>
        [LocalizedDescription("Force")]
        [Symbol("F")]
        Force,
        /// <summary>
        /// The weight of an object is the force acting on the object due to gravity.
        /// </summary>
        [LocalizedDescription("Weight")]
        [Symbol("W")]
        Weight,
        /// <summary>
        /// Work is the energy transferred to or from an object via the application of force along a displacement. 
        /// In its simplest form, it is often represented as the product of force and displacement. 
        /// A force is said to do positive work if (when applied) it has a component in the direction of the displacement of the point of application. 
        /// A force does negative work if it has a component opposite to the direction of the displacement at the point of application of the force.
        /// </summary>
        [LocalizedDescription("Work")]
        [Symbol("W")]
        Work,
        /// <summary>
        /// Heat is energy in transfer to or from a thermodynamic system, by mechanisms other than thermodynamic work or transfer of matter.
        /// </summary>
        [LocalizedDescription("Heat")]
        [Symbol("Q")]
        Heat,
        /// <summary>
        /// Pressure (symbol: p or P) is the force applied perpendicular to the surface of an object per unit area over which that force is distributed.
        /// </summary>
        [LocalizedDescription("Pressure")]
        [Symbol("P")]
        Pressure,
        /// <summary>
        /// Stress is a physical quantity that expresses the internal forces that neighbouring particles of a continuous material exert on each other, while strain is the measure of the deformation of the material.
        /// </summary>
        [LocalizedDescription("Stress")]
        [Symbol("σ")]
        Stress,
        /// <summary>
        /// Power is the amount of energy transferred or converted per unit time. 
        /// In the International System of Units, the unit of power is the watt, equal to one joule per second. 
        /// In older works, power is sometimes called activity.
        /// Power is a scalar quantity.
        /// </summary>
        [LocalizedDescription("Power")]
        [Symbol("P")]
        Power,
        /// <summary>
        /// Frequency is the number of occurrences of a repeating event per unit of time.
        /// It is also occasionally referred to as temporal frequency to emphasize the contrast to spatial frequency, and ordinary frequency to emphasize the contrast to angular frequency.
        /// Frequency is measured in hertz (Hz) which is equal to one event per second. 
        /// The period is the duration of time of one cycle in a repeating event, so the period is the reciprocal of the frequency.
        /// </summary>
        [LocalizedDescription("Frequency")]
        [Symbol("f")]
        Frequency,
        /// <summary>
        /// Area is the quantity that expresses the extent of a two-dimensional region, shape, or planar lamina, in the plane.
        /// Surface area is its analog on the two-dimensional surface of a three-dimensional object.
        /// </summary>
        [LocalizedDescription("Area")]
        [Symbol("A")]
        Area,
        /// <summary>
        ///  the speed (commonly referred to as v) of an object is the magnitude of the rate of change of its position with time or the magnitude of the change of its position per unit of time.
        ///  It is thus a scalar quantity.
        /// </summary>
        [LocalizedDescription("Speed")]
        [Symbol("v")]
        Speed,
        /// <summary>
        /// The velocity of an object is the rate of change of its position with respect to a frame of reference, and is a function of time. 
        /// Velocity is equivalent to a specification of an object's speed and direction of motion (e.g. 60 km/h to the north).
        /// </summary>
        [LocalizedDescription("Velocity")]
        [Symbol("v")]
        Velocity,
        /// <summary>
        /// Acceleration is the rate of change of the velocity of an object with respect to time. 
        /// Accelerations are vector quantities (in that they have magnitude and direction).
        /// </summary>
        [LocalizedDescription("Acceleration")]
        [Symbol("a")]
        Acceleration,
        /// <summary>
        /// The density (more precisely, the volumetric mass density; also known as specific mass), of a substance is its mass per unit volume.
        /// </summary>
        [LocalizedDescription("Density")]
        [Symbol("ρ")]
        Density
    }
}
