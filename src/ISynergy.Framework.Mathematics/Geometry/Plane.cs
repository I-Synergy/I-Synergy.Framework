﻿using ISynergy.Framework.Mathematics.Vectors;

namespace ISynergy.Framework.Mathematics.Geometry;

/// <summary>
///   3D Plane class with normal vector and distance from origin.
/// </summary>
/// 
[Serializable]
public class Plane : IEquatable<Plane>, IFormattable
{
    [NonSerialized]
    private Vector3 _normal;
    private float _offset;

    /// <summary>
    ///   Creates a new <see cref="Plane"/> object
    ///   passing through the <see cref="Point3.Origin"/>.
    /// </summary>
    /// 
    /// <param name="a">The first component of the plane's normal vector.</param>
    /// <param name="b">The second component of the plane's normal vector.</param>
    /// <param name="c">The third component of the plane's normal vector.</param>
    /// 
    public Plane(float a, float b, float c)
    {
        _normal = new Vector3(a, b, c);
    }

    /// <summary>
    ///   Creates a new <see cref="Plane"/> object
    ///   passing through the <see cref="Point3.Origin"/>.
    /// </summary>
    /// 
    /// <param name="normal">The plane's normal vector.</param>
    /// 
    public Plane(Vector3 normal)
    {
        _normal = normal;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Plane"/> class.
    /// </summary>
    /// 
    /// <param name="a">The first component of the plane's normal vector.</param>
    /// <param name="b">The second component of the plane's normal vector.</param>
    /// <param name="c">The third component of the plane's normal vector.</param>
    /// 
    /// <param name="offset">The distance from the plane to the origin.</param>
    /// 
    public Plane(float a, float b, float c, float offset)
    {
        _normal = new Vector3(a, b, c);
        _offset = offset;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Plane"/> class.
    /// </summary>
    /// 
    /// <param name="normal">The plane's normal vector.</param>
    /// <param name="offset">The distance from the plane to the origin.</param>
    /// 
    public Plane(Vector3 normal, float offset)
    {
        _normal = normal;
        _offset = offset;
    }
    /// <summary>
    ///   Constructs a new <see cref="Plane"/> object from three points.
    /// </summary>
    /// 
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <param name="point3">The third point.</param>
    /// 
    /// <returns>A <see cref="Plane"/> passing through the three points.</returns>
    /// 
    public static Plane FromPoints(Point3 point1, Point3 point2, Point3 point3)
    {
        float x2m1 = point2.X - point1.X;
        float y2m1 = point2.Y - point1.Y;
        float z2m1 = point2.Z - point1.Z;

        float x3m1 = point3.X - point1.X;
        float y3m1 = point3.Y - point1.Y;
        float z3m1 = point3.Z - point1.Z;

        float a = (y2m1 * z3m1) - (z2m1 * y3m1);
        float b = (z2m1 * x3m1) - (x2m1 * z3m1);
        float c = (x2m1 * y3m1) - (y2m1 * x3m1);

        float d = -(a * point1.X + b * point1.Y + c * point1.Z);

        return new Plane(a, b, c, d);
    }

    /// <summary>
    ///   Gets the plane's normal vector.
    /// </summary>
    /// 
    public Vector3 Normal
    {
        get { return _normal; }
    }

    /// <summary>
    ///   Gets or sets the constant <c>a</c> in the plane
    ///   definition <c>a * x + b * y + c * z + d = 0</c>.
    /// </summary>
    /// 
    public float A
    {
        get { return _normal.X; }
        set { _normal.X = value; }
    }

    /// <summary>
    ///   Gets or sets the constant <c>b</c> in the plane
    ///   definition <c>a * x + b * y + c * z + d = 0</c>.
    /// </summary>
    /// 
    public float B
    {
        get { return _normal.Y; }
        set { _normal.Y = value; }
    }

    /// <summary>
    ///   Gets or sets the constant <c>c</c> in the plane
    ///   definition <c>a * x + b * y + c * z + d = 0</c>.
    /// </summary>
    /// 
    public float C
    {
        get { return _normal.Z; }
        set { _normal.Z = value; }
    }

    /// <summary>
    ///   Gets or sets the distance offset 
    ///   between the plane and the origin.
    /// </summary>
    /// 
    public float Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }
    /// <summary>
    ///   Computes the distance from point to plane.
    /// </summary>
    /// 
    /// <param name="point">The point to have its distance from the plane computed.</param>
    /// 
    /// <returns>The distance from <paramref name="point"/> to this plane.</returns>
    /// 
    public double DistanceToPoint(Point3 point)
    {
        float a = _normal.X;
        float b = _normal.Y;
        float c = _normal.Z;

        double num = Math.Abs(a * point.X + b * point.Y + c * point.Z + _offset);
        double den = Math.Sqrt(a * a + b * b + c * c);

        return num / den;
    }

    /// <summary>
    ///   Normalizes this plane by dividing its components
    ///   by the <see cref="Normal"/> vector's norm.
    /// </summary>
    /// 
    public void Normalize()
    {
        float norm = _normal.Normalize();
        _offset /= norm;
    }

    /// <summary>
    ///   Implements the operator !=.
    /// </summary>
    /// 
    public static bool operator ==(Plane a, Plane b)
    {
        if ((object)a is null && (object)b is null)
            return true;
        if ((object)a is null || (object)b is null)
            return false;

        return a._offset == b._offset && a._normal == b._normal;
    }

    /// <summary>
    ///   Implements the operator !=.
    /// </summary>
    /// 
    public static bool operator !=(Plane a, Plane b)
    {
        if ((object)a is null && (object)b is null)
            return false;
        if ((object)a is null || (object)b is null)
            return true;

        return a._offset != b._offset || a._normal != b._normal;
    }

    /// <summary>
    ///   Determines whether the specified <see cref="Plane"/> is equal to this instance.
    /// </summary>
    /// 
    /// <param name="other">The <see cref="Plane"/> to compare with this instance.</param>
    /// <param name="tolerance">The acceptance tolerance threshold to consider the instances equal.</param>
    /// 
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// 
    public bool Equals(Plane other, double tolerance)
    {
        return (Math.Abs(_offset - other._offset) < tolerance)
            && (Math.Abs(_normal.X - other._normal.X) < tolerance)
            && (Math.Abs(_normal.Y - other._normal.Y) < tolerance)
            && (Math.Abs(_normal.Z - other._normal.Z) < tolerance);
    }

    /// <summary>
    ///   Determines whether the specified <see cref="Plane"/> is equal to this instance.
    /// </summary>
    /// 
    /// <param name="other">The <see cref="Plane"/> to compare with this instance.</param>
    /// 
    /// <returns>
    ///   <c>true</c> if the specified <see cref="Plane"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// 
    public bool Equals(Plane other)
    {
        return _offset == other._offset && _normal == other._normal;
    }

    /// <summary>
    ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// 
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// 
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// 
    public override bool Equals(object obj)
    {
        Plane other = obj as Plane;
        if (other is null)
            return false;

        return Equals(other);
    }

    /// <summary>
    ///   Returns a hash code for this instance.
    /// </summary>
    /// 
    /// <returns>
    ///   A hash code for this instance, suitable for use in hashing 
    ///   algorithms and data structures like a hash table. 
    /// </returns>
    /// 
    public override int GetHashCode()
    {
        return _offset.GetHashCode() + 13 * _normal.GetHashCode();
    }
    /// <summary>
    ///   Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// 
    /// <returns>
    ///   A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// 
    public override string ToString()
    {
        return ToString("g", System.Globalization.CultureInfo.CurrentCulture);
    }

    /// <summary>
    ///   Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// 
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// 
    /// <returns>
    ///   A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// 
    public string ToString(string format, IFormatProvider formatProvider)
    {
        var f = new Formatter();
        f.format = format;
        f.provider = formatProvider;

        return String.Format("{0}x {1}y {2}z {3} = 0",
            f.s(A), f.s(B), f.s(C), f.s(Offset));
    }

    /// <summary>
    ///   Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// 
    /// <param name="variable">The variable to put on the left hand side. Can
    ///   be either 'x', 'y' or 'z'.</param>
    /// 
    /// <returns>
    ///   A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// 
    public string ToString(char variable)
    {
        return ToString(variable, System.Globalization.CultureInfo.CurrentCulture);
    }

    /// <summary>
    ///   Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// 
    /// <param name="variable">The variable to put on the left hand side. Can
    ///   be either 'x', 'y' or 'z'.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// 
    /// <returns>
    ///   A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// 
    public string ToString(char variable, IFormatProvider formatProvider)
    {
        var f = new Formatter();
        f.provider = formatProvider;

        switch (variable)
        {
            case 'x':
                return String.Format("x = {0}y {1}z {2}",
              f.s(-B / A), f.s(-C / A), f.s(-Offset / A));

            case 'y':
                return String.Format("y = {0}x {1}z {2}",
              f.s(-A / B), f.s(-C / B), f.s(-Offset / B));

            case 'z':
                return String.Format("z = {0}x {1}y {2}",
              f.s(-A / C), f.s(-B / C), f.s(-Offset / C));

            default:
                throw new FormatException();
        }
    }

    private class Formatter
    {
        public IFormatProvider provider;
        public string format = "g";

        public string s(float x)
        {
            if (provider is null)
                provider = System.Globalization.CultureInfo.CurrentCulture;

            string str = x.ToString(format, provider);
            return (x > 0) ? "+" + str : str;
        }
    }
}
