using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Matrices;



using ISynergy.Framework.Mathematics.Abstractions;
namespace ISynergy.Framework.Mathematics.Decompositions;

/// <summary>
///     Gram-Schmidt Orthogonalization.
/// </summary>
public class GramSchmidtOrthogonalization
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GramSchmidtOrthogonalization" /> class.
    /// </summary>
    /// <param name="value">The matrix <c>A</c> to be decomposed.</param>
    public GramSchmidtOrthogonalization(Matrix<double> value)
        : this(value, true)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GramSchmidtOrthogonalization" /> class.
    /// </summary>
    /// <param name="value">The matrix <c>A</c> to be decomposed.</param>
    /// <param name="modified">
    ///     True to use modified Gram-Schmidt; false
    ///     otherwise. Default is true (and is the recommended setup).
    /// </param>
    public GramSchmidtOrthogonalization(Matrix<double> value, bool modified)
    {
        var matrix = value.Array;

        if (matrix.GetLength(0) != matrix.GetLength(1))
            throw new DimensionMismatchException(nameof(value), "Matrix must be square.");

        var size = matrix.GetLength(0);

        OrthogonalFactor = new double[size, size];
        UpperTriangularFactor = new double[size, size];

        if (modified)
            for (var j = 0; j < size; j++)
            {
                var v = matrix.GetColumn(j);

                for (var i = 0; i < j; i++)
                {
                    UpperTriangularFactor[i, j] = OrthogonalFactor.GetColumn(i).Dot(v);
                    var t = UpperTriangularFactor[i, j].Multiply(OrthogonalFactor.GetColumn(i));
                    v.Subtract(t, result: v);
                }

                UpperTriangularFactor[j, j] = v.Euclidean();
                OrthogonalFactor.SetColumn(j, v.Divide(UpperTriangularFactor[j, j]));
            }

        else
            for (var j = 0; j < size; j++)
            {
                var v = matrix.GetColumn(j);
                var a = matrix.GetColumn(j);

                for (var i = 0; i < j; i++)
                {
                    UpperTriangularFactor[i, j] = OrthogonalFactor.GetColumn(j).Dot(a);
                    v = v.Subtract(UpperTriangularFactor[i, j].Multiply(OrthogonalFactor.GetColumn(i)));
                }

                UpperTriangularFactor[j, j] = v.Euclidean();
                OrthogonalFactor.SetColumn(j, v.Divide(UpperTriangularFactor[j, j]));
            }
    }
    /// <summary>
    ///     Returns the orthogonal factor matrix <c>Q</c>.
    /// </summary>
    public double[,] OrthogonalFactor { get; }

    /// <summary>
    ///     Returns the upper triangular factor matrix <c>R</c>.
    /// </summary>
    public double[,] UpperTriangularFactor { get; }
}