using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Matrices;

namespace ISynergy.Framework.Mathematics.Vectors;

public static partial class Vector
{
    /// <summary>
    ///     Gets the inner product (scalar product) between two vectors (a'*b).
    /// </summary>
    /// <param name="a">A vector.</param>
    /// <param name="b">A vector.</param>
    /// <returns>The inner product of the multiplication of the vectors.</returns>
    public static double Dot(this Sparse<double> a, Sparse<double> b)
    {
        double sum = 0;

        int i = 0, j = 0;

        while (i < a.Indices.Length && j < b.Indices.Length)
        {
            int posx = a.Indices[i];
            int posy = b.Indices[j];

            if (posx == posy)
            {
                sum += a.Values[i] * b.Values[j];
                i++;
                j++;
            }
            else if (posx < posy)
            {
                i++;
            }
            else //if (posx > posy)
            {
                j++;
            }
        }

        return sum;
    }

    /// <summary>
    ///     Gets the inner product (scalar product) between two vectors (a'*b).
    /// </summary>
    /// <param name="a">A vector.</param>
    /// <param name="b">A vector.</param>
    /// <returns>The inner product of the multiplication of the vectors.</returns>
    public static double Dot(this Sparse<double> a, double[] b)
    {
        double sum = 0;

        int i = 0, j = 0;

        while (i < a.Indices.Length && j < b.Length)
        {
            int posx = a.Indices[i];
            var posy = j;

            if (posx == posy)
            {
                sum += a.Values[i] * b[j];
                i++;
                j++;
            }
            else if (posx < posy)
            {
                i++;
            }
            else // if (posx > posy)
            {
                j++;
            }
        }

        return sum;
    }

    /// <summary>
    ///     Adds a sparse vector to a dense vector.
    /// </summary>
    public static double[] Add(this Sparse<double> a, double[] b, double[] result)
    {
        for (var j = 0; j < b.Length; j++)
            result[j] = b[j];

        for (var j = 0; j < a.Indices.Length; j++)
            result[a.Indices[j]] += a.Values[j];

        return result;
    }

    /// <summary>
    ///     Divides an array of sparse vectors by the associated scalars in a dense vector.
    /// </summary>
    public static Sparse<double>[] Divide(this Sparse<double>[] a, double[] b, Sparse<double>[] result)
    {
        for (var i = 0; i < a.Length; i++)
            Elementwise.Divide(a[i].Values, b[i], result[i].Values);
        return result;
    }

    /// <summary>
    ///     Divides a sparse vector by a scalar.
    /// </summary>
    public static Sparse<double> Divide(this Sparse<double> a, double b, Sparse<double> result)
    {
        Elementwise.Divide(a.Values, b, result.Values);
        return result;
    }
}