using ISynergy.Framework.Mathematics.Distances;
using System;
using System.Reflection;

namespace ISynergy.Framework.Mathematics
{
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
        /// <example>
        ///     <code source="tests\ISynergy.Framework.Mathematics.Tests.Math\DistanceTest.cs" region="doc_getdistance" />
        /// </example>
        public static IDistance<T> GetDistance<T>(Func<T, T, double> func)
        {
#if NETSTANDARD1_4
            var methods = typeof(Distance).GetTypeInfo().DeclaredMethods.Where(m=>m.IsPublic && m.IsStatic);
#else
            var methods = typeof(Distance).GetMethods(BindingFlags.Public | BindingFlags.Static);
#endif
            foreach (var method in methods)
            {
#if NETSTANDARD1_4
                var methodInfo = func.GetMethodInfo();
#else
                var methodInfo = func.Method;
#endif
                if (methodInfo == method)
                {
                    var t = Type.GetType("ISynergy.Framework.Mathematics.Distances." + method.Name);

                    if (t == null)
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
    }
}