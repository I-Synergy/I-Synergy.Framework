using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Matrices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Common;

/// <summary>
///     Static class Norm. Defines a set of extension methods defining norms measures.
/// </summary>
public static partial class Norm
{
    /// <summary>
    ///     Returns the maximum column sum of the given matrix.
    /// </summary>
    public static double Norm1(this double[,] a)
    {
        double[] columnSums = Matrix.Sum(a, 1);
        return Matrix.Max(columnSums);
    }

    /// <summary>
    ///     Returns the maximum column sum of the given matrix.
    /// </summary>
    public static double Norm1(this double[][] a)
    {
        double[] columnSums = Matrix.Sum(a, 1);
        return Matrix.Max(columnSums);
    }

    /// <summary>
    ///     Returns the maximum singular value of the given matrix.
    /// </summary>
    public static double Norm2(this double[,] a)
    {
        return new SingularValueDecomposition(a, false, false).TwoNorm;
    }

    /// <summary>
    ///     Returns the maximum singular value of the given matrix.
    /// </summary>
    public static double Norm2(this double[][] a)
    {
        return new JaggedSingularValueDecomposition(a, false, false).TwoNorm;
    }
}