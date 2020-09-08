using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Represents a structure that defines a location (X, Y) in a two-dimensional space.
    /// </summary>
    [DebuggerDisplay("{X}, {Y}")]
    public struct Point
    {
        /// <summary>
        /// A <see cref="Point" /> instance which X and Y values are set to 0.
        /// </summary>
        public static readonly Point Empty = new Point(0, 0);

        /// <summary>
        /// The X-coordinate of the point.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(X))]
        public double X;

        /// <summary>
        /// The Y-coordinate of the point.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(Y))]
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Determines whether two <see cref="Point" /> structures are equal.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        /// <summary>
        /// Determines whether two <see cref="Size" /> structures are not equal.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Rounds the X and Y members of the specified <see cref="Point" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Point.</returns>
        public static Point Round(Point point)
        {
            point.X = System.Math.Round(point.X);
            point.Y = System.Math.Round(point.Y);

            return point;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Point radPoint)
            {
                return radPoint == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
