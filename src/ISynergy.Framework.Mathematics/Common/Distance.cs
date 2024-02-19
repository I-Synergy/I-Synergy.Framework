using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Distances;
using ISynergy.Framework.Mathematics.Distances.Base;
using System.Reflection;

namespace ISynergy.Framework.Mathematics.Common;

/// <summary>
///     Static class Distance. Defines a set of extension methods defining distance measures.
/// </summary>
public static partial class Distance
{
    /// <summary>
    ///     Checks whether a function is a real metric distance, i.e. respects
    ///     the triangle inequality. Please note that a function can still pass
    ///     this test and not respect the triangle inequality.
    /// </summary>
    public static bool IsMetric(Func<double[], double[], double> value)
    {
        // Direct test
        var z = value(new[] { 1.0 }, new[] { 1.0 });
        if (z > 2 || z < 0)
            return false;
        var a = new double[1];
        var b = new double[1];

        for (var i = -10; i < 10; i++)
        {
            a[0] = i;

            for (var j = -10; j < +10; j++)
            {
                b[0] = j;
                var c = value(a, b);

                if (c > Math.Abs(i) + Math.Abs(j))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Checks whether a function is a real metric distance, i.e. respects
    ///     the triangle inequality. Please note that a function can still pass
    ///     this test and not respect the triangle inequality.
    /// </summary>
    public static bool IsMetric<T>(IDistance<T> value)
    {
        return value is IMetric<T>;
    }

    /// <summary>
    ///     Gets the a <see cref="IDistance{T}" /> object implementing a
    ///     particular method of the <see cref="Distance" /> static class.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is intended to be used in scenarios where you have been using any
    ///         of the static methods in the <see cref="Distance" /> class, but now you would like
    ///         to obtain a reference to an object that implements the same distance you have been
    ///         using before, but in a object-oriented, polymorphic manner. Please see the example
    ///         below for more details.
    ///     </para>
    ///     <para>
    ///         Note: This method relies on reflection and might not work
    ///         on all scenarios, environments, and/or platforms.
    ///     </para>
    /// </remarks>
    /// <typeparam name="T">The type of the elements being compared in the distance function.</typeparam>
    /// <param name="func">The method of <see cref="Distance" />.</param>
    /// <returns>
    ///     An object of the class that implements the given distance.
    /// </returns>
    public static IDistance<T> GetDistance<T>(Func<T, T, double> func)
    {
        var methods = typeof(Distance).GetMethods(BindingFlags.Public | BindingFlags.Static);

        foreach (var method in methods)
        {
            var methodInfo = func.Method;

            if (methodInfo == method)
            {
                var t = Type.GetType("ISynergy.Framework.Mathematics.Distances." + method.Name);

                if (t is null)
                    // TODO: Remove the following special case, as it is needed only
                    // for preserving compatibility for a few next releases more.
                    if (methodInfo.Name == "BitwiseHamming")
                        return new Hamming() as IDistance<T>;

                return (IDistance<T>)Activator.CreateInstance(t, new object[] { });
            }
        }

        return null;
    }

    /// <summary>
    ///     Gets the Levenshtein distance between two points.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>The Levenshtein distance between x and y.</returns>
    public static double Levenshtein<T>(T[] x, T[] y)
    {
        return new Levenshtein<T>().Distance(x, y);
    }

    /// <summary>
    ///   Gets the Yule distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Yule distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Yule"/> documentation page.
    /// </example>
    ///
    public static double Yule(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheYule.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Jaccard distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Jaccard distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Jaccard"/> documentation page.
    /// </example>
    ///
    public static double Jaccard(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheJaccard.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Hellinger distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Hellinger distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Hellinger"/> documentation page.
    /// </example>
    ///
    public static double Hellinger(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheHellinger.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Euclidean"/> documentation page.
    /// </example>
    ///
    public static double Euclidean(double x, double y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Euclidean"/> documentation page.
    /// </example>
    ///
    public static double Euclidean(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="vector1x"></param>
    /// <param name="vector1y"></param>
    /// <param name="vector2x"></param>
    /// <param name="vector2y"></param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Euclidean"/> documentation page.
    /// </example>
    ///
    public static double Euclidean(double vector1x, double vector1y, double vector2x, double vector2y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheEuclidean.Distance(vector1x, vector1y, vector2x, vector2y);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Euclidean"/> documentation page.
    /// </example>
    ///
    public static double Euclidean(Tuple<double, double> x, Tuple<double, double> y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Euclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Euclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Euclidean"/> documentation page.
    /// </example>
    ///
    public static double Euclidean(Sparse<double> x, Sparse<double> y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareMahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="chol"></param>
    /// 
    /// <returns>The SquareMahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareMahalanobis"/> documentation page.
    /// </example>
    ///
    public static double SquareMahalanobis(double[] x, double[] y, CholeskyDecomposition chol)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new SquareMahalanobis(chol).Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareMahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="svd"></param>
    /// 
    /// <returns>The SquareMahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareMahalanobis"/> documentation page.
    /// </example>
    ///
    public static double SquareMahalanobis(double[] x, double[] y, SingularValueDecomposition svd)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new SquareMahalanobis(svd).Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareMahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="precision"></param>
    /// 
    /// <returns>The SquareMahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareMahalanobis"/> documentation page.
    /// </example>
    ///
    public static double SquareMahalanobis(double[] x, double[] y, double[,] precision)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new SquareMahalanobis(precision).Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareMahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SquareMahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareMahalanobis"/> documentation page.
    /// </example>
    ///
    public static double SquareMahalanobis(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSquareMahalanobis.Distance(x, y);
    }

    /// <summary>
    ///   Gets the RusselRao distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The RusselRao distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.RusselRao"/> documentation page.
    /// </example>
    ///
    public static double RusselRao(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheRusselRao.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Chebyshev distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Chebyshev distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Chebyshev"/> documentation page.
    /// </example>
    ///
    public static double Chebyshev(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheChebyshev.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Dice distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Dice distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Dice"/> documentation page.
    /// </example>
    ///
    public static double Dice(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheDice.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SokalMichener distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SokalMichener distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SokalMichener"/> documentation page.
    /// </example>
    ///
    public static double SokalMichener(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSokalMichener.Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="dimensions"></param>
    /// 
    /// <returns>The WeightedEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedEuclidean(double[] x, double[] y, int dimensions)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new WeightedEuclidean(dimensions).Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="weights"></param>
    /// 
    /// <returns>The WeightedEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedEuclidean(double[] x, double[] y, double[] weights)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new WeightedEuclidean(weights).Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The WeightedEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedEuclidean(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheWeightedEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Angular distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Angular distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Angular"/> documentation page.
    /// </example>
    ///
    public static double Angular(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheAngular.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double SquareEuclidean(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSquareEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double SquareEuclidean(double x, double y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSquareEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double SquareEuclidean(Sparse<double> x, Sparse<double> y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSquareEuclidean.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// 
    /// <returns>The SquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double SquareEuclidean(double x1, double y1, double x2, double y2)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSquareEuclidean.Distance(x1, y1, x2, y2);
    }

    /// <summary>
    ///   Gets the Hamming distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Hamming distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Hamming"/> documentation page.
    /// </example>
    ///
    public static double Hamming(double[] x, double[] y)
    {
        return cacheHamming.Distance(x, y);
    }

    /// <summary>
    ///   Gets the ArgMax distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The ArgMax distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.ArgMax"/> documentation page.
    /// </example>
    ///
    public static double ArgMax(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheArgMax.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Modular distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="modulo"></param>
    /// 
    /// <returns>The Modular distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Modular"/> documentation page.
    /// </example>
    ///
    public static double Modular(double x, double y, int modulo)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new Modular(modulo).Distance(x, y);
    }

    /// <summary>
    ///   Gets the Modular distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Modular distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Modular"/> documentation page.
    /// </example>
    ///
    public static double Modular(double x, double y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheModular.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Cosine distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Cosine distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Cosine"/> documentation page.
    /// </example>
    ///
    public static double Cosine(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheCosine.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Mahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="chol"></param>
    /// 
    /// <returns>The Mahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Mahalanobis"/> documentation page.
    /// </example>
    ///
    public static double Mahalanobis(double[] x, double[] y, CholeskyDecomposition chol)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new Mahalanobis(chol).Distance(x, y);
    }

    /// <summary>
    ///   Gets the Mahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="svd"></param>
    /// 
    /// <returns>The Mahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Mahalanobis"/> documentation page.
    /// </example>
    ///
    public static double Mahalanobis(double[] x, double[] y, SingularValueDecomposition svd)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new Mahalanobis(svd).Distance(x, y);
    }

    /// <summary>
    ///   Gets the Mahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="precision"></param>
    /// 
    /// <returns>The Mahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Mahalanobis"/> documentation page.
    /// </example>
    ///
    public static double Mahalanobis(double[] x, double[] y, double[,] precision)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new Mahalanobis(precision).Distance(x, y);
    }

    /// <summary>
    ///   Gets the Mahalanobis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Mahalanobis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Mahalanobis"/> documentation page.
    /// </example>
    ///
    public static double Mahalanobis(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheMahalanobis.Distance(x, y);
    }

    /// <summary>
    ///   Gets the BrayCurtis distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The BrayCurtis distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.BrayCurtis"/> documentation page.
    /// </example>
    ///
    public static double BrayCurtis(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheBrayCurtis.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Minkowski distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="p"></param>
    /// 
    /// <returns>The Minkowski distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Minkowski"/> documentation page.
    /// </example>
    ///
    public static double Minkowski(double[] x, double[] y, double p)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new Minkowski(p).Distance(x, y);
    }

    /// <summary>
    ///   Gets the Minkowski distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Minkowski distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Minkowski"/> documentation page.
    /// </example>
    ///
    public static double Minkowski(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheMinkowski.Distance(x, y);
    }

    /// <summary>
    ///   Gets the SokalSneath distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The SokalSneath distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.SokalSneath"/> documentation page.
    /// </example>
    ///
    public static double SokalSneath(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheSokalSneath.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Matching distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Matching distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Matching"/> documentation page.
    /// </example>
    ///
    public static double Matching(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheMatching.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Canberra distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Canberra distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Canberra"/> documentation page.
    /// </example>
    ///
    public static double Canberra(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheCanberra.Distance(x, y);
    }

    /// <summary>
    ///   Gets the RogersTanimoto distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The RogersTanimoto distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.RogersTanimoto"/> documentation page.
    /// </example>
    ///
    public static double RogersTanimoto(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheRogersTanimoto.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Manhattan distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Manhattan distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Manhattan"/> documentation page.
    /// </example>
    ///
    public static double Manhattan(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheManhattan.Distance(x, y);
    }

    /// <summary>
    ///   Gets the Kulczynski distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The Kulczynski distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.Kulczynski"/> documentation page.
    /// </example>
    ///
    public static double Kulczynski(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheKulczynski.Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedSquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="dimensions"></param>
    /// 
    /// <returns>The WeightedSquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedSquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedSquareEuclidean(double[] x, double[] y, int dimensions)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new WeightedSquareEuclidean(dimensions).Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedSquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// <param name="weights"></param>
    /// 
    /// <returns>The WeightedSquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedSquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedSquareEuclidean(double[] x, double[] y, double[] weights)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return new WeightedSquareEuclidean(weights).Distance(x, y);
    }

    /// <summary>
    ///   Gets the WeightedSquareEuclidean distance between two points.
    /// </summary>
    ///  
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>The WeightedSquareEuclidean distance between x and y.</returns>
    /// 
    /// <example>
    ///   For examples, please see <see cref="ISynergy.Framework.Mathematics.Distances.WeightedSquareEuclidean"/> documentation page.
    /// </example>
    ///
    public static double WeightedSquareEuclidean(double[] x, double[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheWeightedSquareEuclidean.Distance(x, y);
    }
}