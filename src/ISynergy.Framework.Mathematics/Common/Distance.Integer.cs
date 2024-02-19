using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mathematics.Distances;

namespace ISynergy.Framework.Mathematics.Common;

public static partial class Distance
{
    /// <summary>
    ///     Checks whether a function is a real metric distance, i.e. respects
    ///     the triangle inequality. Please note that a function can still pass
    ///     this test and not respect the triangle inequality.
    /// </summary>
    public static bool IsMetric(Func<int[], int[], double> value)
    {
        // Direct test
        var z = value(new[] { 1 }, new[] { 1 });
        if (z > 2 || z < 0)
            return false;

        var size = 3;
        var zero = new int[size];

        foreach (var a in Combinatorics.Sequences(3, size, true))
            foreach (var b in Combinatorics.Sequences(3, size, true))
            {
                var dza = value(zero, a);
                var dzb = value(zero, b);
                var dab = value(a, b);

                if (dab > dza + dzb)
                    return false;

                var daz = value(a, zero);
                var dbz = value(b, zero);
                var dba = value(b, a);

                if (daz != dza || dbz != dzb || dab != dba)
                    return false;
            }

        return true;
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
    public static double Yule(int[] x, int[] y) => Yule(x.ToDoubleArray(), y.ToDoubleArray());

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
    public static double RusselRao(int[] x, int[] y) => RusselRao(x.ToDoubleArray(), y.ToDoubleArray());

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
    public static double Dice(int[] x, int[] y) => Dice(x.ToDoubleArray(), y.ToDoubleArray());
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
    public static double SokalMichener(int[] x, int[] y) => SokalMichener(x.ToDoubleArray(), y.ToDoubleArray());

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
    public static double Modular(int x, int y, int modulo)
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
    public static double Modular(int x, int y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheModular.Distance(x, y);
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
    public static double Minkowski(int[] x, int[] y, double p)
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
    public static double Minkowski(int[] x, int[] y)
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
    public static double SokalSneath(int[] x, int[] y)
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
    public static double Matching(int[] x, int[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheMatching.Distance(x, y);
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
    public static double RogersTanimoto(int[] x, int[] y)
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
    public static double Manhattan(int[] x, int[] y)
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
    public static double Kulczynski(int[] x, int[] y)
    {
        // Note: this is an auto-generated method stub that forwards the call
        // to the actual implementation, indicated in the next line below:
        return cacheKulczynski.Distance(x, y);
    }

}
