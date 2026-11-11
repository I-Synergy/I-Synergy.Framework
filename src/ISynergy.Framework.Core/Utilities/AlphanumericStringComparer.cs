using ISynergy.Framework.Core.Extensions;
using System.Collections;
using System.Text;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class AlphanumericStringComparer.
/// Implements the <see cref="IComparer" />
/// </summary>
/// <seealso cref="IComparer" />
public class AlphanumericStringComparer : IComparer
{
    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    private static List<string> GetList(string source)
    {
        var result = new List<string>();
        var flag = char.IsDigit(source[0]);
        var item = new StringBuilder();

        foreach (var character in source.EnsureNotNull())
        {
            if (flag != char.IsDigit(character) || character == '\'')
            {
                if (!string.IsNullOrEmpty(item.ToString()))
                    result.Add(item.ToString());

                item.Clear();
                flag = char.IsDigit(character);
            }

            if (char.IsDigit(character) || char.IsLetter(character))
            {
                item.Append(character);
            }
        }

        result.Add(item.ToString());
        return result;
    }

    /// <summary>
    /// Compares the specified x.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>System.Int32.</returns>
    public int Compare(object? x, object? y)
    {
        if (x is not string s1 || y is not string s2)
        {
            return 0;
        }

        if (s1 == s2)
        {
            return 0;
        }

        // Walk through two the strings with two markers.
        var str1 = GetList(s1);
        var str2 = GetList(s2);

        PadToSameLength(str1, str2);

        for (var i = 0; i < str1.Count && i < str2.Count; i++)
        {
            var result = CompareSegments(str1[i], str2[i]);

            if (result != 0)
                return result;
        }

        return 0;
    }

    /// <summary>
    /// Pads both segment lists with empty segments so that they have the same number of segments.
    /// </summary>
    /// <param name="str1">The segments of the first string.</param>
    /// <param name="str2">The segments of the second string.</param>
    private static void PadToSameLength(List<string> str1, List<string> str2)
    {
        while (str1.Count != str2.Count)
        {
            if (str1.Count < str2.Count)
            {
                str1.Add("");
            }
            else
            {
                str2.Add("");
            }
        }
    }

    /// <summary>
    /// Compares a single pair of segments, treating numeric segments as numbers.
    /// </summary>
    /// <param name="segment1">The segment of the first string.</param>
    /// <param name="segment2">The segment of the second string.</param>
    /// <returns>A signed value indicating the relative order of the two segments.</returns>
    private static int CompareSegments(string segment1, string segment2)
    {
        var is1Integer = int.TryParse(segment1, out var intX);
        var is2Integer = int.TryParse(segment2, out var intY);

        // Both are strings, or only one of them is a number: compare them as text.
        if (!is1Integer || !is2Integer)
        {
            return segment1.CompareTo(segment2);
        }

        // Both are numbers.
        if (intX == intY)
        {
            return CompareSameValueNumericSegments(segment1, segment2);
        }

        return CompareLeadingZeros(segment1, segment2, intX, intY);
    }

    /// <summary>
    /// Compares two numeric segments that represent the same value (for example "01" and "1").
    /// </summary>
    /// <param name="segment1">The segment of the first string.</param>
    /// <param name="segment2">The segment of the second string.</param>
    /// <returns>A signed value indicating the relative order of the two segments.</returns>
    private static int CompareSameValueNumericSegments(string segment1, string segment2)
    {
        if (segment1.Length < segment2.Length)
            return 1;

        if (segment1.Length > segment2.Length)
            return -1;

        return 0;
    }

    /// <summary>
    /// Compares two numeric segments with different values, taking leading zeros into account.
    /// </summary>
    /// <param name="segment1">The segment of the first string.</param>
    /// <param name="segment2">The segment of the second string.</param>
    /// <param name="intX">The numeric value of the first segment.</param>
    /// <param name="intY">The numeric value of the second segment.</param>
    /// <returns>A signed value indicating the relative order of the two segments.</returns>
    private static int CompareLeadingZeros(string segment1, string segment2, int intX, int intY)
    {
        var st1ZeroCount = segment1.Trim().Length - segment1.TrimStart('0').Length;
        var st2ZeroCount = segment2.Trim().Length - segment2.TrimStart('0').Length;

        if (st1ZeroCount > st2ZeroCount)
            return -1;

        if (st1ZeroCount < st2ZeroCount)
            return 1;

        return intX.CompareTo(intY);
    }
}
