using System;

namespace ISynergy.Framework.Mathematics.Kinematics
{
    /// <summary>
    ///     Denavit Hartenberg joint-description parameters.
    /// </summary>
    [Serializable]
    public class DenavitHartenbergParameters
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DenavitHartenbergParameters" /> class.
        /// </summary>
        /// <param name="alpha">Angle (in radians) of the Z axis relative to the last joint.</param>
        /// <param name="theta">Angle (in radians) of the X axis relative to the last joint.</param>
        /// <param name="radius">Length or radius of the joint.</param>
        /// <param name="offset">Offset along Z axis relatively to the last joint.</param>
        public DenavitHartenbergParameters(double alpha, double theta, double radius, double offset)
        {
            Alpha = alpha;
            Theta = theta;
            Radius = (float)radius;
            Offset = (float)offset;
        }

        /// <summary>
        ///     Denavit Hartenberg parameters constructor
        /// </summary>
        public DenavitHartenbergParameters()
        {
        }

        /// <summary>
        ///     Angle in radians about common normal, from
        ///     old <c>z</c> axis to the new <c>z</c> axis.
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        ///     Angle in radians about previous <c>z</c>,
        ///     from old <c>x</c> to the new <c>x</c>.
        /// </summary>
        public double Theta { get; set; }

        /// <summary>
        ///     Length of the joint (also known as <c>a</c>).
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        ///     Offset along previous <c>z</c> to the common normal (also known as <c>d</c>).
        /// </summary>
        public double Offset { get; set; }
    }
}