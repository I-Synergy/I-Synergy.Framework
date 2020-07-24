using System;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts.Helpers
{
    /// <summary>
    /// Class RadialHelpers.
    /// </summary>
    internal static class RadialHelpers
    {
        /// <summary>
        /// The pi
        /// </summary>
        public const float PI = (float)Math.PI;

        /// <summary>
        /// The upright angle
        /// </summary>
        private const float UprightAngle = PI / 2f;

        /// <summary>
        /// The total angle
        /// </summary>
        private const float TotalAngle = 2f * PI;


        /// <summary>
        /// Gets the circle point.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>SKPoint.</returns>
        public static SKPoint GetCirclePoint(float r, float angle)
        {
            return new SKPoint(r * (float)Math.Cos(angle), r * (float)Math.Sin(angle));
        }

        /// <summary>
        /// Creates the sector path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="outerRadius">The outer radius.</param>
        /// <param name="innerRadius">The inner radius.</param>
        /// <param name="margin">The margin.</param>
        /// <returns>SKPath.</returns>
        public static SKPath CreateSectorPath(float start, float end, float outerRadius, float innerRadius = 0.0f, float margin = 0.0f)
        {
            var path = new SKPath();

            // if the sector has no size, then it has no path
            if (start == end)
            {
                return path;
            }

            // the the sector is a full circle, then do that
            if (end - start == 1.0f)
            {
                path.AddCircle(0, 0, outerRadius, SKPathDirection.Clockwise);
                path.AddCircle(0, 0, innerRadius, SKPathDirection.Clockwise);
                path.FillType = SKPathFillType.EvenOdd;
                return path;
            }

            // calculate the angles
            var startAngle = (TotalAngle * start) - UprightAngle;
            var endAngle = (TotalAngle * end) - UprightAngle;
            var large = endAngle - startAngle > PI ? SKPathArcSize.Large : SKPathArcSize.Small;
            
            var sectorCenterAngle = ((endAngle - startAngle) / 2f) + startAngle;
            var sectorCenterRadius = ((outerRadius - innerRadius) / 2f) + innerRadius;

            // calculate the angle for the margins
            var offsetR = outerRadius == 0 ? 0 : ((margin / (TotalAngle * outerRadius)) * TotalAngle);
            var offsetr = innerRadius == 0 ? 0 : ((margin / (TotalAngle * innerRadius)) * TotalAngle);

            // get the points
            var a = GetCirclePoint(outerRadius, startAngle + offsetR);
            var b = GetCirclePoint(outerRadius, endAngle - offsetR);
            var c = GetCirclePoint(innerRadius, endAngle - offsetr);
            var d = GetCirclePoint(innerRadius, startAngle + offsetr);

            // add the points to the path
            path.MoveTo(a);
            path.ArcTo(outerRadius, outerRadius, 0, large, SKPathDirection.Clockwise, b.X, b.Y);
            path.LineTo(c);

            if (innerRadius == 0.0f)
            {
                // take a short cut
                path.LineTo(d);
            }
            else
            {
                path.ArcTo(innerRadius, innerRadius, 0, large, SKPathDirection.CounterClockwise, d.X, d.Y);
            }

            path.Close();

            return path;
        }
    }
}
