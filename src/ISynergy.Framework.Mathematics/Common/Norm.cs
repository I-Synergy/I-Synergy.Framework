using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Abstractions;



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
        return new SingularValueDecomposition(new Matrix<double>(a), false, false).TwoNorm;
    }

    /// <summary>
    ///     Returns the maximum singular value of the given matrix.
    /// </summary>
    public static double Norm2(this double[][] a)
    {
        return new JaggedSingularValueDecomposition(new Jagged<double>(a), false, false).TwoNorm;
    }
}