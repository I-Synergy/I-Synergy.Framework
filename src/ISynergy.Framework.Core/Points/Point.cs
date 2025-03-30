using System.Globalization;

namespace ISynergy.Framework.Core.Points;

/// <summary>
/// Structure for representing a pair of coordinates of double, decimal, double or int type.
/// </summary>
/// <remarks><para>The structure is used to store a pair of numeric point coordinates with single precision.</para>
/// <para>Sample usage:</para>
/// <code>
/// // assigning coordinates in the constructor
/// Point p1 = new Point( 10, 20 );
/// // creating a point and assigning coordinates later
/// Point p2;
/// p2.X = 30;
/// p2.Y = 40;
/// // calculating distance between two points
/// double distance = p1.DistanceTo( p2 );
/// </code>
/// </remarks>
[Serializable]
public class Point : IComparable<Point>
{
    /// <summary> 
    /// X coordinate.
    /// </summary> 
    /// 
    public double X { get; private set; }

    /// <summary> 
    /// Y coordinate.
    /// </summary> 
    /// 
    public double Y { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public Point(double x, double y, bool round = false, int decimals = 0)
    {
        if (round)
        {
            X = Math.Round(x, decimals);
            Y = Math.Round(y, decimals);
        }
        else
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public Point(int x, int y, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(x), Convert.ToDouble(y), round, decimals)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public Point(long x, long y, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(x), Convert.ToDouble(y), round, decimals)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public Point(decimal x, decimal y, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(x), Convert.ToDouble(y), round, decimals)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public Point(float x, float y, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(x), Convert.ToDouble(y), round, decimals)
    {
    }

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// 
    /// <returns>Returns Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    /// 
    public double DistanceTo(Point anotherPoint)
    {
        var dx = Convert.ToDouble(X) - Convert.ToDouble(anotherPoint.X);
        var dy = Convert.ToDouble(Y) - Convert.ToDouble(anotherPoint.Y);

        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Calculate squared Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// 
    /// <returns>Returns squared Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    /// 
    public double SquaredDistanceTo(Point anotherPoint)
    {
        var dx = Convert.ToDouble(X) - Convert.ToDouble(anotherPoint.X);
        var dy = Convert.ToDouble(Y) - Convert.ToDouble(anotherPoint.Y);

        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static Point operator +(Point point1, Point point2)
    {
        return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static Point Add(Point point1, Point point2)
    {
        return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static Point operator -(Point point1, Point point2)
    {
        return new Point(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static Point Subtract(Point point1, Point point2)
    {
        return new Point(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static Point operator +(Point point, double valueToAdd)
    {
        return new Point(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static Point Add(Point point, double valueToAdd)
    {
        return new Point(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static Point operator -(Point point, double valueToSubtract)
    {
        return new Point(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static Point Subtract(Point point, double valueToSubtract)
    {
        return new Point(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static Point operator *(Point point, double factor)
    {
        return new Point(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static Point Multiply(Point point, double factor)
    {
        return new Point(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static Point operator /(Point point, double factor)
    {
        return new Point(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static Point Divide(Point point, double factor)
    {
        return new Point(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Equality operator - checks if two points have equal coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are equal.</returns>
    ///
    public static bool operator ==(Point point1, Point point2)
    {
        return ((point1.X == point2.X) && (point1.Y == point2.Y));
    }

    /// <summary>
    /// Inequality operator - checks if two points have different coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are not equal.</returns>
    ///
    public static bool operator !=(Point point1, Point point2)
    {
        return ((point1.X != point2.X) || (point1.Y != point2.Y));
    }

    /// <summary>
    /// Check if this instance of <see cref="Point"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another point to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object? obj)
    {
        return (obj is Point) ? (this == (Point)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the point in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
    }

    /// <summary>
    /// Calculate Euclidean norm of the vector comprised of the point's 
    /// coordinates - distance from (0, 0) in other words.
    /// </summary>
    /// 
    /// <returns>Returns point's distance from (0, 0) point.</returns>
    /// 
    public double EuclideanNorm(bool round = false, int decimals = 0)
    {
        var result = Math.Sqrt(X * X + Y * Y);

        if (round)
            return Math.Round(result, decimals);

        return result;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.</returns>
    public int CompareTo(Point? other)
    {
        var line = this.Y.CompareTo(other!.Y);

        if (line == 0)
            return this.X.CompareTo(other.X);

        return line;
    }
}
