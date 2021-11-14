namespace ISynergy.Framework.Geography.Global
{
    /// <summary>
    /// Encapsulates a three dimensional location on a globe (GlobalCoordinates combined with
    /// an elevation in meters above a reference ellipsoid).
    /// </summary>
    public struct GlobalPosition : IComparable<GlobalPosition>, IEquatable<GlobalPosition>
    {
        /// <summary>
        /// The precision
        /// </summary>
        private const double Precision = 0.000000000001;

        /// <summary>
        /// Creates a new instance of GlobalPosition.
        /// </summary>
        /// <param name="coordinates">coordinates on the reference ellipsoid.</param>
        /// <param name="elevation">elevation, in meters, above the reference ellipsoid.</param>
        public GlobalPosition(GlobalCoordinates coordinates, double elevation)
        {
            Coordinates = coordinates;
            Elevation = elevation;
        }

        /// <summary>
        /// Creates a new instance of GlobalPosition for a position on the surface of
        /// the reference ellipsoid.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public GlobalPosition(GlobalCoordinates coordinates)
            : this(coordinates, 0.0)
        {
        }

        /// <summary>
        /// Get/set global coordinates.
        /// </summary>
        public GlobalCoordinates Coordinates;

        /// <summary>
        /// Get/set latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public Angle Latitude
        {
            get { return Coordinates.Latitude; }
            set { Coordinates.Latitude = value; }
        }

        /// <summary>
        /// Get/set longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public Angle Longitude
        {
            get { return Coordinates.Longitude; }
            set { Coordinates.Longitude = value; }
        }

        /// <summary>
        /// Get/set elevation, in meters, above the surface of the reference ellipsoid.
        /// </summary>
        /// <value>The elevation.</value>
        public double Elevation { get; set; }

        /// <summary>
        /// Compare this position to another.  Western longitudes are less than
        /// eastern logitudes.  If longitudes are equal, then southern latitudes are
        /// less than northern latitudes.  If coordinates are equal, lower elevations
        /// are less than higher elevations
        /// </summary>
        /// <param name="other">instance to compare to</param>
        /// <returns>-1, 0, or +1 as per IComparable contract</returns>
        public int CompareTo(GlobalPosition other)
        {
            var retval = Coordinates.CompareTo(other.Coordinates);

            if (retval != 0)
                return retval;

            if (Elevation.IsApproximatelyEqual(other.Elevation, Precision)) retval = 0;
            else if (Elevation.IsSmaller(other.Elevation, Precision)) retval = -1;
            else retval = 1;

            return retval;
        }

        /// <summary>
        /// Calculate a hash code.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hash = Coordinates.GetHashCode();

            if (Elevation != 0) hash *= (int)Elevation;

            return hash;
        }

        /// <summary>
        /// Compare this position to another position for equality
        /// </summary>
        /// <param name="other">Another GlobalPosition to compare to</param>
        /// <returns>true if both are equal</returns>
        public bool Equals(GlobalPosition other)
        {
            return Elevation.IsApproximatelyEqual(other.Elevation, Precision)
                   && Coordinates.Equals(other.Coordinates);
        }

        /// <summary>
        /// Compare this position to another object for equality.
        /// </summary>
        /// <param name="obj">Another object to compare to</param>
        /// <returns>true if the obj is a GlobalPosition and is the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is GlobalPosition position)
            {
                return ((IEquatable<GlobalPosition>)this).Equals(position);
            }

            return false;
        }

        /// <summary>
        /// Get position as a culture invariant string.
        /// </summary>
        /// <returns>The position as culture invariant string</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Coordinates);
            builder.Append(Elevation.ToString(NumberFormatInfo.InvariantInfo));
            builder.Append("m");

            return builder.ToString();
        }

        #region Operators
        /// <summary>
        /// Test two GlobalPositions for equality
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if they are the same</returns>
        public static bool operator ==(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        /// <summary>
        /// Test two GlobalPositions for equality
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if they are the same</returns>
        public static bool operator !=(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) != 0;
        }

        /// <summary>
        /// Test whether a GlobalPosition is smaller than another one
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if the first position is smaller than the second</returns>
        public static bool operator <(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Test whether a GlobalPosition is smaller or equal than another one
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if the first position is smaller than or equal to the second</returns>
        public static bool operator <=(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Test whether a GlobalPosition is greater than another one
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if the first position is greater than the second</returns>
        public static bool operator >(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        /// Test whether a GlobalPosition is greater or equal than another one
        /// </summary>
        /// <param name="lhs">The first position</param>
        /// <param name="rhs">The second position</param>
        /// <returns>True if the first position is greater than or equal to the second</returns>
        public static bool operator >=(GlobalPosition lhs, GlobalPosition rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }
        #endregion
    }
}
