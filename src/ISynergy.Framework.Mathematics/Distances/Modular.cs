namespace ISynergy.Framework.Mathematics.Distances
{
    using System;

    /// <summary>
    ///   Modular distance (shortest distance between two marks on a circle).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The modular distance is the distance of two member in a circular space,
    ///   or a ring. Roughly, it can be understood as the minimum difference between
    ///   two numbers in this circular space, either going “clockwise” or “counter-clockwise”.
    ///   For example, in a circle, the modular distance between 1° and 359° is 2° 
    ///   (and not 358°).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://crsouza.com/2009/09/modulo-and-modular-distance-in-c">
    ///       http://crsouza.com/2009/09/modulo-and-modular-distance-in-c </a></description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    [Serializable]
    public struct Modular : IDistance<double>, IDistance<int>, ICloneable
    {
        int modulo;

        /// <summary>
        ///   Gets the maximum value that the distance can
        ///   have before it wraps around in the circle.
        /// </summary>
        /// 
        public int Modulo { get { return modulo; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Modular"/> class.
        /// </summary>
        /// 
        /// <param name="modulo">The maximum value that the distance can
        ///   have before it wraps around in the circle (i.e. 360).</param>
        /// 
        public Modular(int modulo)
        {
            this.modulo = modulo;
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(double x, double y)
        {
            return Math.Min(Tools.Mod(x - y, modulo), Tools.Mod(y - x, modulo));
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(int x, int y)
        {
            return Math.Min(Tools.Mod(x - y, modulo), Tools.Mod(y - x, modulo));
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Modular(modulo);
        }
    }
}
