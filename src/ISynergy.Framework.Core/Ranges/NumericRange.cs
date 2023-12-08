using System.Collections;

namespace ISynergy.Framework.Core.Ranges;

/// <summary>
///   Represents a double range with minimum and maximum values,
///   where values are single-precision double numbers.
/// </summary>
/// 
/// <remarks>
///   This class represents a double range with inclusive limits, where
///   both minimum and maximum values of the range are included into it.
///   Mathematical notation of such range is <b>[min, max]</b>.
/// </remarks>
/// 
/// <example>
/// <code>
/// // create [0.25, 1.5] range
/// var range1 = new SingleRange(0.25f, 1.5f);
/// 
/// // create [1.00, 2.25] range
/// var range2 = new SingleRange(1.00f, 2.25f);
/// 
/// // check if values is inside of the first range
/// if (range1.IsInside(0.75f))
/// {
///     // ...
/// }
/// 
/// // check if the second range is inside of the first range
/// if (range1.IsInside(range2))
/// {
///     // ...
/// }
/// 
/// // check if two ranges overlap
/// if (range1.IsOverlapping(range2))
/// {
///     // ...
/// }
/// </code>
/// </example>
[Serializable]
public struct NumericRange : IEquatable<NumericRange>, IEnumerable<double>
{
    /// <summary>
    ///   Minimum value of the range.
    /// </summary>
    /// 
    /// <remarks>
    ///   Represents minimum value (left side limit) of the range [<b>min</b>, max].
    /// </remarks>
    /// 
    public double Min { get; set; }

    /// <summary>
    ///   Maximum value of the range.
    /// </summary>
    /// 
    /// <remarks>
    ///   Represents maximum value (right side limit) of the range [min, <b>max</b>].
    /// </remarks>
    /// 
    public double Max { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(double min, double max, bool round = false, int decimals = 0)
    {
        if (round)
        {
            Min = Math.Round(min, decimals);
            Max = Math.Round(max, decimals);
        }
        else
        {
            Min = min;
            Max = max;
        }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(int min, int max, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(min), Convert.ToDouble(max), round, decimals)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(long min, long max, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(min), Convert.ToDouble(max), round, decimals)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(float min, float max, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(min), Convert.ToDouble(max), round, decimals)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(decimal min, decimal max, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(min), Convert.ToDouble(max), round, decimals)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericRange"/> class.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// <param name="round"></param>
    /// <param name="decimals"></param>
    /// 
    public NumericRange(byte min, byte max, bool round = false, int decimals = 0)
        : this(Convert.ToDouble(min), Convert.ToDouble(max), round, decimals)
    {
    }

    /// <summary>
    ///   Gets the length of the range, defined as (max - min).
    /// </summary>
    /// 
    public double Length
    {
        get { return Max - Min; }
    }

    /// <summary>
    ///   Check if the specified value is inside of the range.
    /// </summary>
    /// 
    /// <param name="x">Value to check.</param>
    /// 
    /// <returns>
    ///   <b>True</b> if the specified value is inside of the range or <b>false</b> otherwise.
    /// </returns>
    /// 
    public bool IsInside(double x)
    {
        return ((x >= Min) && (x <= Max));
    }

    /// <summary>
    ///   Check if the specified range is inside of the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check.</param>
    /// 
    /// <returns>
    ///   <b>True</b> if the specified range is inside of the range or <b>false</b> otherwise.
    /// </returns>
    /// 
    public bool IsInside(NumericRange range)
    {
        return ((IsInside(range.Min)) && (IsInside(range.Max)));
    }

    /// <summary>
    ///   Check if the specified range overlaps with the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check for overlapping.</param>
    /// 
    /// <returns>
    ///   <b>True</b> if the specified range overlaps with the range or <b>false</b> otherwise.
    /// </returns>
    /// 
    public bool IsOverlapping(NumericRange range)
    {
        return ((IsInside(range.Min)) || (IsInside(range.Max)) ||
                 (range.IsInside(Min)) || (range.IsInside(Max)));
    }

    /// <summary>
    ///   Computes the intersection between two ranges.
    /// </summary>
    /// 
    /// <param name="range">The second range for which the intersection should be calculated.</param>
    /// 
    /// <returns>An new <see cref="NumericRange"/> structure containing the intersection
    /// between this range and the <paramref name="range"/> given as argument.</returns>
    /// 
    public NumericRange Intersection(NumericRange range)
    {
        return new NumericRange(Math.Max(Min, range.Min), Math.Min(Max, range.Max));
    }

    /// <summary>
    ///   Determines whether two instances are equal.
    /// </summary>
    /// 
    public static bool operator ==(NumericRange range1, NumericRange range2)
    {
        return ((range1.Min == range2.Min) && (range1.Max == range2.Max));
    }

    /// <summary>
    ///   Determines whether two instances are not equal.
    /// </summary>
    /// 
    public static bool operator !=(NumericRange range1, NumericRange range2)
    {
        return ((range1.Min != range2.Min) || (range1.Max != range2.Max));
    }

    /// <summary>
    ///   Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// 
    /// <param name="other">An object to compare with this object.</param>
    /// 
    /// <returns>
    ///   true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// 
    public bool Equals(NumericRange other)
    {
        return this == other;
    }

    /// <summary>
    ///   Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// 
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// 
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is NumericRange) ? (this == (NumericRange)obj) : false;
    }

    /// <summary>
    ///   Returns a hash code for this instance.
    /// </summary>
    /// 
    /// <returns>
    ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    /// 
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + Min.GetHashCode();
            hash = hash * 31 + Max.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    ///   Returns a <see cref="String" /> that represents this instance.
    /// </summary>
    /// 
    /// <returns>
    ///   A <see cref="String" /> that represents this instance.
    /// </returns>
    /// 
    public override string ToString()
    {
        return String.Format("[{0}, {1}]", Min, Max);
    }

    /// <summary>
    ///   Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// 
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// 
    /// <returns>
    ///   A <see cref="System.String" /> that represents this instance.
    /// </returns>
    /// 
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return String.Format("[{0}, {1}]",
            Min.ToString(format, formatProvider),
            Max.ToString(format, formatProvider));
    }


    /// <summary>
    ///   Converts this single-precision range into an <see cref="NumericRange"/>.
    /// </summary>
    /// 
    /// <param name="provideInnerRange">
    ///   Specifies if inner integer range must be returned or outer range.</param>
    /// 
    /// <returns>Returns integer version of the range.</returns>
    /// 
    /// <remarks>
    ///   If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
    ///   returned integer range will always fit inside of the current single precision range.
    ///   If it is set to <see langword="false"/>, then current single precision range will always
    ///   fit into the returned integer range.
    /// </remarks>
    ///
    public NumericRange ToRange(bool provideInnerRange)
    {
        double min;
        double max;

        if (provideInnerRange)
        {
            min = Math.Ceiling(Min);
            max = Math.Floor(Max);
        }
        else
        {
            min = Math.Floor(Min);
            max = Math.Ceiling(Max);
        }

        return new NumericRange(min, max);
    }

    /// <summary>
    ///   Returns an enumerator that iterates through a collection.
    /// </summary>
    /// 
    /// <returns>
    ///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    /// 
    public IEnumerator<double> GetEnumerator()
    {
        for (var i = Min; i < Max; i++)
            yield return i;
    }

    /// <summary>
    ///   Returns an enumerator that iterates through a collection.
    /// </summary>
    /// 
    /// <returns>
    ///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    /// 
    IEnumerator IEnumerable.GetEnumerator()
    {
        for (var i = Min; i < Max; i++)
            yield return i;
    }
}
