using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Provides static mathematical functions and constants.
    /// </summary>
    public static class Mathematics
    {
        /// <summary>
        /// The factor used to convert degrees to their radians equivalent.
        /// </summary>
        public const double DegToRadFactor = Math.PI / 180;

        /// <summary>
        /// The factor used to convert radians to their degree equivalent.
        /// </summary>
        public const double RadToDegFactor = 180 / Math.PI;

        /// <summary>
        /// Smallest unit such that 1.0+DBL_EPSILON != 1.0.
        /// </summary>
        public const double Epsilon = 2.2204460492503131e-9;

        /// <summary>
        /// Determines whether the specified value is close to 0 within the order of EPSILON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is zero; otherwise, <c>false</c>.</returns>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Determines whether the specified value is close to 0 within the order of EPSILON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is zero; otherwise, <c>false</c>.</returns>
        public static bool IsZero(decimal value)
        {
            return IsZero((double)value);
        }

        /// <summary>
        /// Determines whether the specified value is close to 1 within the order of EPSILON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is one; otherwise, <c>false</c>.</returns>
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Determines whether the specified value is close to 1 within the order of EPSILON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is one; otherwise, <c>false</c>.</returns>
        public static bool IsOne(decimal value)
        {
            return IsOne((double)value);
        }

        /// <summary>
        /// Determines whether the two specified values are close within the order of EPSILON.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AreClose(double value1, double value2)
        {
            return AreClose(value1, value2, Epsilon);
        }

        /// <summary>
        /// Determines whether the two specified values are close within the order of <paramref name="epsilon" />.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AreClose(double value1, double value2, double epsilon)
        {
            // in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
            var delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Gets the distance between two points in a plane.
        /// </summary>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        /// <returns>System.Double.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public static double GetPointDistance(double x1, double x2, double y1, double y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;

            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Gets the point that lies on the arc segment of the ellipse, described by the center and radius parameters.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>Point.</returns>
        public static Point GetArcPoint(double angle, Point center, double radius)
        {
            var angleInRad = angle * DegToRadFactor;

            var x = center.X + (Math.Cos(angleInRad) * radius);
            var y = center.Y + (Math.Sin(angleInRad) * radius);

            return new Point(x, y);
        }

        /// <summary>
        /// Converts cartesian into polar coordinates.
        /// </summary>
        /// <param name="point">The point we are converting.</param>
        /// <param name="centerPoint">The (0,0) point of the the coordinate system.</param>
        /// <param name="reverse">True to reverse the calculated angle using the (360 - angle) expression, false otherwise.</param>
        /// <returns>Coordinates as radius and angle (in degrees).</returns>
        internal static Tuple<double, double> ToPolarCoordinates(Point point, Point centerPoint, bool reverse = false)
        {
            var xOffset = point.X - centerPoint.X;
            var yLength = Math.Abs(point.Y - centerPoint.Y);

            var pointRadius = Math.Sqrt(xOffset * xOffset + yLength * yLength);

            var pointAngle = Math.Asin(yLength / pointRadius) * 180 / Math.PI;

            // Determine quadrant and adjust the point angle accordingly
            if (centerPoint.X < point.X && centerPoint.Y > point.Y)
            {
                // I quadrant
                pointAngle = 360 - pointAngle;
            }
            else if (centerPoint.X >= point.X && centerPoint.Y > point.Y)
            {
                // II quadrant
                pointAngle += 180;
            }
            else if (centerPoint.X >= point.X && centerPoint.Y <= point.Y)
            {
                // III quadrant
                pointAngle = 180 - pointAngle;
            }

            if (reverse)
            {
                pointAngle = (360 - pointAngle) % 360;
            }

            return new Tuple<double, double>(pointRadius, pointAngle);
        }

        /// <summary>
        /// Converts to cartesiancoordinates.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="angleDeg">The angle deg.</param>
        /// <returns>Point.</returns>
        /// <autogeneratedoc />
        internal static Point ToCartesianCoordinates(double radius, double angleDeg)
        {
            var x = radius * Math.Cos(angleDeg / 180 * Math.PI);
            var y = radius * Math.Sin(angleDeg / 180 * Math.PI);

            return new Point(x, y);
        }

        /// <summary>
        /// Coerces the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Double.</returns>
        /// <autogeneratedoc />
        internal static double CoerceValue(double value, double min, double max)
        {
            return Math.Min(Math.Max(min, value), max);
        }

        /// <summary>
        /// Calculates the intersection y.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="verticalLineX">The vertical line x.</param>
        /// <returns>System.Double.</returns>
        /// <autogeneratedoc />
        internal static double CalculateIntersectionY(double x1, double y1, double x2, double y2, double verticalLineX)
        {
            return (verticalLineX - x1) * (y2 - y1) / (x2 - x1) + y1;
        }

        /// <summary>
        /// Calculates the intersection x.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="horizontalLineY">The horizontal line y.</param>
        /// <returns>System.Double.</returns>
        /// <autogeneratedoc />
        internal static double CalculateIntersectionX(double x1, double y1, double x2, double y2, double horizontalLineY)
        {
            return (horizontalLineY - y1) * (x2 - x1) / (y2 - y1) + x1;
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>T.</returns>
        /// <autogeneratedoc />
        internal static T Clamp<T>(T value, T min, T max)
        {
            var comparer = Comparer<T>.Default;

            var c = comparer.Compare(value, min);
            if (c < 0)
            {
                return min;
            }

            c = comparer.Compare(value, max);
            if (c > 0)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Checks if number is even or not.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Boolean</returns>
        public static bool IsEven(int number)
        {
            if (number % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
