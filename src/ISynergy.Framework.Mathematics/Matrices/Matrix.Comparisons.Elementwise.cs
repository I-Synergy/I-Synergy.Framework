using ISynergy.Framework.Mathematics.Abstractions;
using System.Runtime.InteropServices;


namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Elementwise
{
    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool[] Equals(Double[] a, Double[] b, Double atol = 0, Double rtol = 0)
    {
        bool[] r = VectorCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];

                if (ExactComparisons.AreEqual(A ,B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);

                    if (C == 0 && delta <= rtol)
                    {
                        r[i] = true;
                    }
                    else if (D == 0 && delta <= rtol)
                    {
                        r[i] = true;
                    }
                    else
                    {
                        r[i] = (delta <= Math.Abs(C) * rtol);
                    }
                }
            }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];

                if (ExactComparisons.AreEqual(A ,B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;

                    r[i] = (Math.Abs(C - D) <= atol);
                }
            }
        }
        else
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];

                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    r[i] = (ExactComparisons.AreEqual(A ,B));
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Matrix<Double> a, Matrix<Double> b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        bool[,] r = MatrixCreateAs<Double, bool>(aArray);

        if (aArray.Length == 0)
            return r;

        var spanA = MemoryMarshal.CreateSpan(ref aArray[0, 0], aArray.Length);
        var spanB = MemoryMarshal.CreateSpan(ref bArray[0, 0], bArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref r[0, 0], r.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];

                if (ExactComparisons.AreEqual(A ,B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);

                    if (C == 0 && delta <= rtol)
                    {
                        spanR[i] = true;
                    }
                    else if (D == 0 && delta <= rtol)
                    {
                        spanR[i] = true;
                    }
                    else
                    {
                        spanR[i] = (delta <= Math.Abs(C) * rtol);
                    }
                }
            }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];

                if (ExactComparisons.AreEqual(A ,B))
                {
                    spanR[i] = true; continue;
                }

                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true; continue;
                }
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false; continue;
                }
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false; continue;
                }
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false; continue;
                }
                spanR[i] = (Math.Abs(A - B) <= atol);
            }
        }
        else
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];

                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true; continue;
                }

                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false; continue;
                }
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false; continue;
                }
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false; continue;
                }
                spanR[i] = (ExactComparisons.AreEqual(A ,B));
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Matrix<Double> a, Jagged<Double> b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        bool[][] r = JaggedCreateAs<Double, bool>(aArray);

        if (rtol > 0)
        {
            for (var i = 0; i < bArray.Length; i++)
                for (var j = 0; j < bArray[i].Length; j++)
                {
                    var A = aArray[i, j];
                    var B = bArray[i][j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);

                        if (C == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else if (D == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else
                        {
                            r[i][j] = (delta <= Math.Abs(C) * rtol);
                        }
                    }
                }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < bArray.Length; i++)
                for (var j = 0; j < bArray[i].Length; j++)
                {
                    var A = aArray[i, j];
                    var B = bArray[i][j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;
                        r[i][j] = (Math.Abs(C - D) <= atol);
                    }
                }
        }
        else
        {
            for (var i = 0; i < bArray.Length; i++)
                for (var j = 0; j < bArray[i].Length; j++)
                {
                    var A = aArray[i, j];
                    var B = bArray[i][j];

                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        r[i][j] = (ExactComparisons.AreEqual(A ,B));
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Jagged<Double> a, Matrix<Double> b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        bool[][] r = JaggedCreateAs<Double, bool>(aArray);

        if (rtol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i, j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);

                        if (C == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else if (D == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else
                        {
                            r[i][j] = (delta <= Math.Abs(C) * rtol);
                        }
                    }
                }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i, j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;

                        r[i][j] = (Math.Abs(C - D) <= atol);
                    }
                }
        }
        else
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i, j];

                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        r[i][j] = (ExactComparisons.AreEqual(A ,B));
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Jagged<Double> a, Jagged<Double> b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        bool[][] r = JaggedCreateAs<Double, bool>(aArray);

        if (rtol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i][j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);

                        if (C == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else if (D == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else
                        {
                            r[i][j] = (delta <= Math.Abs(C) * rtol);
                        }
                    }
                }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i][j];

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;

                        r[i][j] = (Math.Abs(C - D) <= atol);
                    }
                }
        }
        else
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = bArray[i][j];

                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        r[i][j] = (ExactComparisons.AreEqual(A ,B));
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool[] Equals(Double[] a, Double b, Double atol = 0, Double rtol = 0)
    {
        bool[] r = VectorCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;

                if (ExactComparisons.AreEqual(A ,B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);

                    if (C == 0 && delta <= rtol)
                    {
                        r[i] = true;
                    }
                    else if (D == 0 && delta <= rtol)
                    {
                        r[i] = true;
                    }
                    else
                    {
                        r[i] = (delta <= Math.Abs(C) * rtol);
                    }
                }
            }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;

                if (ExactComparisons.AreEqual(A ,B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;

                    r[i] = (Math.Abs(C - D) <= atol);
                }
            }
        }
        else
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;

                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    r[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    r[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    r[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    r[i] = false;
                }
                else
                {
                    r[i] = (ExactComparisons.AreEqual(A ,B));
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Matrix<Double> a, Double b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        bool[,] r = MatrixCreateAs<Double, bool>(aArray);

        if (aArray.Length == 0)
            return r;

        var spanA = MemoryMarshal.CreateSpan(ref aArray[0, 0], aArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref r[0, 0], r.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;

                if (ExactComparisons.AreEqual(A ,B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);

                    if (C == 0 && delta <= rtol)
                    {
                        spanR[i] = true;
                    }
                    else if (D == 0 && delta <= rtol)
                    {
                        spanR[i] = true;
                    }
                    else
                    {
                        spanR[i] = (delta <= Math.Abs(C) * rtol);
                    }
                }
            }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;

                if (ExactComparisons.AreEqual(A ,B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false;
                }
                else
                {
                    var C = A;
                    var D = B;

                    spanR[i] = (Math.Abs(C - D) <= atol);
                }
            }

        }
        else
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;

                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    spanR[i] = true;
                }
                else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                {
                    spanR[i] = false;
                }
                else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                {
                    spanR[i] = false;
                }
                else
                {
                    spanR[i] = (ExactComparisons.AreEqual(A ,B));
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Jagged<Double> a, Double b, Double atol = 0, Double rtol = 0)
    {
        var aArray = a.Array;
        bool[][] r = JaggedCreateAs<Double, bool>(aArray);

        if (rtol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = b;

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);

                        if (C == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else if (D == 0 && delta <= rtol)
                        {
                            r[i][j] = true;
                        }
                        else
                        {
                            r[i][j] = (delta <= Math.Abs(C) * rtol);
                        }
                    }

                }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = b;

                    if (ExactComparisons.AreEqual(A ,B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        var C = A;
                        var D = B;

                        r[i][j] = (Math.Abs(C - D) <= atol);
                    }
                }
        }
        else
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                {
                    var A = aArray[i][j];
                    var B = b;

                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    {
                        r[i][j] = true;
                    }
                    else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    {
                        r[i][j] = false;
                    }
                    else
                    {
                        r[i][j] = (ExactComparisons.AreEqual(A ,B));
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool[] Equals(Double a, Double[] b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return Equals(left, right, absTolerance, relTolerance);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Double a, Matrix<Double> b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return Equals(left, right, absTolerance, relTolerance);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double a, Jagged<Double> b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return Equals(left, right, absTolerance, relTolerance);
    }
}