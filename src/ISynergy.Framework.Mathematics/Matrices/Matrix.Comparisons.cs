
using System.Runtime.InteropServices;
using ISynergy.Framework.Mathematics.Common;


namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{
    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Double[] a, Double[] b, Double atol = 0, Double rtol = 0)
    {
        if (a == b)
            return true;
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (var i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if ((C == 0 || D == 0) && delta <= rtol)
                    continue;

                if (delta <= Math.Abs(C) * rtol)
                    continue;
                return false;
            }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                if (Math.Abs(C - D) <= atol)
                    continue;
                return false;
            }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                if (ExactComparisons.AreNotEqual(A ,B))
                    return false;
            }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[,] a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        if (a == b)
            return true;
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (var i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (a.Length == 0)
            return true;

        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        var spanB = MemoryMarshal.CreateSpan(ref b[0, 0], b.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if ((C == 0 || D == 0) && delta <= rtol)
                    continue;

                if (delta <= Math.Abs(C) * rtol)
                    continue;
                return false;
            }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                if (Math.Abs(C - D) <= atol)
                    continue;
                return false;
            }

        }
        else
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                if (ExactComparisons.AreNotEqual(A ,B))
                    return false;
            }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[,] a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (var i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (rtol > 0)
        {
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[][] a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (var i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[][] a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        if (a == b)
            return true;
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (var i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Double[] a, Double b, Double atol = 0, Double rtol = 0)
    {
        if (a is null)
            return true;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if ((C == 0 || D == 0) && delta <= rtol)
                    continue;

                if (delta <= Math.Abs(C) * rtol)
                    continue;
                return false;
            }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                if (Math.Abs(C - D) <= atol)
                    continue;
                return false;
            }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                if (ExactComparisons.AreNotEqual(A ,B))
                    return false;
            }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[,] a, Double b, Double atol = 0, Double rtol = 0)
    {
        if (a is null)
            return true;

        if (a.Length == 0)
            return true;

        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if ((C == 0 || D == 0) && delta <= rtol)
                    continue;

                if (delta <= Math.Abs(C) * rtol)
                    continue;
                return false;
            }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;
                if (ExactComparisons.AreEqual(A ,B))
                    continue;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                if (Math.Abs(C - D) <= atol)
                    continue;
                return false;
            }

        }
        else
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    continue;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                if (ExactComparisons.AreNotEqual(A ,B))
                    return false;
            }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double[][] a, Double b, Double atol = 0, Double rtol = 0)
    {
        if (a is null)
            return true;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b;
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }
        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b;
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Double a, Double[] b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return IsEqual(left, right, absTolerance, relTolerance);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return IsEqual(left, right, absTolerance, relTolerance);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        var left = b;
        var right = a;
        var absTolerance = rtol;
        var relTolerance = atol;
        return IsEqual(left, right, absTolerance, relTolerance);
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Double a, Double b, Double atol = 0, Double rtol = 0)
    {
        if (rtol > 0)
        {
                var A__r1 = a;
                var B__r2 = b;
                if (ExactComparisons.AreEqual(A__r1 ,B__r2))
                    return true;
                if (Double.IsNaN(A__r1) && Double.IsNaN(B__r2))
                    return true;
                if (Double.IsNaN(A__r1) ^ Double.IsNaN(B__r2))
                    return false;
                if (Double.IsPositiveInfinity(A__r1) ^ Double.IsPositiveInfinity(B__r2))
                    return false;
                if (Double.IsNegativeInfinity(A__r1) ^ Double.IsNegativeInfinity(B__r2))
                    return false;
                var C__r3 = A__r1;
                var D__r4 = B__r2;
                var delta__r5 = Math.Abs(C__r3 - D__r4);
                if ((C__r3 == 0 || D__r4 == 0) && delta__r5 <= rtol)
                    return true;

                if (delta__r5 <= Math.Abs(C__r3) * rtol)
                    return true;
                return false;

        }

        if (atol > 0)
        {
                var A__r6 = a;
                var B__r7 = b;
                if (ExactComparisons.AreEqual(A__r6 ,B__r7))
                    return true;
                if (Double.IsNaN(A__r6) && Double.IsNaN(B__r7))
                    return true;
                if (Double.IsNaN(A__r6) ^ Double.IsNaN(B__r7))
                    return false;
                if (Double.IsPositiveInfinity(A__r6) ^ Double.IsPositiveInfinity(B__r7))
                    return false;
                if (Double.IsNegativeInfinity(A__r6) ^ Double.IsNegativeInfinity(B__r7))
                    return false;
                var C__r8 = A__r6;
                var D__r9 = B__r7;
                if (Math.Round(Math.Abs(C__r8 - D__r9), Tools.GetDecimalCount(atol)) <= atol)
                    return true;
                return false;

        }
            var A__r10 = a;
            var B__r11 = b;
            if (Double.IsNaN(A__r10) && Double.IsNaN(B__r11))
                return true;
            if (Double.IsNaN(A__r10) ^ Double.IsNaN(B__r11))
                return false;
            if (Double.IsPositiveInfinity(A__r10) ^ Double.IsPositiveInfinity(B__r11))
                return false;
            if (Double.IsNegativeInfinity(A__r10) ^ Double.IsNegativeInfinity(B__r11))
                return false;
            if (ExactComparisons.AreNotEqual(A__r10 ,B__r11))
                return false;

        return true;
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Int32[] a, Int32[] b, Int32 atol = 0, Double rtol = 0)
    {
        if (a == b)
            return true;
        if (a is null && b is null)
            return true;
        if (a is null ^ b is null)
            return false;
        int[] la = a.GetLength(true);
        int[] lb = b.GetLength(true);
        if (la.Length != lb.Length)
            return false;
        for (int i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        if (rtol > 0)
        {
            for (int i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (A == B)
                    continue;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if ((C == 0 || D == 0) && delta <= rtol)
                    continue;

                if (delta <= Math.Abs(C) * rtol)
                    continue;
                return false;
            }

        }
        else if (atol > 0)
        {
            for (int i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (A == B)
                    continue;
                var C = A;
                var D = B;
                if (Math.Abs(C - D) <= atol)
                    continue;
                return false;
            }

        }
        else
        {
            for (int i = 0; i < a.Length; i++)
            {
                var A = a[i];
                var B = b[i];
                if (A != B)
                    return false;
            }

        }

        return true;
    }

    /// <summary>
    ///   Determines a matrix is symmetric.
    /// </summary>
    ///
    public static bool IsSymmetric(this Double[][] a, Double atol = 0, Double rtol = 0)
    {
        if (!a.IsSquare())
            return false;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = a[j][i];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = a[j][i];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = a[j][i];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }

    /// <summary>
    ///   Determines a matrix is symmetric.
    /// </summary>
    ///
    public static bool IsSymmetric(this Double[,] a, Double atol = 0, Double rtol = 0)
    {
        if (!a.IsSquare())
            return false;

        if (rtol > 0)
        {
            for (var i = 0; i < a.Rows(); i++)
                for (var j = 0; j < i; j++)
                {
                    var A = a[i, j];
                    var B = a[j, i];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if ((C == 0 || D == 0) && delta <= rtol)
                        continue;

                    if (delta <= Math.Abs(C) * rtol)
                        continue;
                    return false;
                }

        }
        else if (atol > 0)
        {
            for (var i = 0; i < a.Rows(); i++)
                for (var j = 0; j < i; j++)
                {
                    var A = a[i, j];
                    var B = a[j, i];
                    if (ExactComparisons.AreEqual(A ,B))
                        continue;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    var C = A;
                    var D = B;
                    if (Math.Abs(C - D) <= atol)
                        continue;
                    return false;
                }

        }
        else
        {
            for (var i = 0; i < a.Rows(); i++)
                for (var j = 0; j < i; j++)
                {
                    var A = a[i, j];
                    var B = a[j, i];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                        continue;
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        return false;
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        return false;
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        return false;
                    if (ExactComparisons.AreNotEqual(A ,B))
                        return false;
                }

        }

        return true;
    }
}