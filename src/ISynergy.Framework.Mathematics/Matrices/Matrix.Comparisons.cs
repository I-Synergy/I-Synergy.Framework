
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
                if (A == B)
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
                if (C == 0)
                {
                    if (delta <= rtol)
                        continue;
                }
                else if (D == 0)
                {
                    if (delta <= rtol)
                        continue;
                }

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
                if (A == B)
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
                if (A != B)
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

        unsafe
        {
            fixed (Double* ptrA = a)
            fixed (Double* ptrB = b)
            {
                if (rtol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (A == B)
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
                        if (C == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }

                        if (delta <= Math.Abs(C) * rtol)
                            continue;
                        return false;
                    }

                }
                else if (atol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (A == B)
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
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                            continue;
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            return false;
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            return false;
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            return false;
                        if (A != B)
                            return false;
                    }

                }
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A != B)
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A != B)
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A != B)
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
                if (A == B)
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
                if (C == 0)
                {
                    if (delta <= rtol)
                        continue;
                }
                else if (D == 0)
                {
                    if (delta <= rtol)
                        continue;
                }

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
                if (A == B)
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
                if (A != B)
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

        unsafe
        {
            fixed (Double* ptrA = a)
            {
                if (rtol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = b;
                        if (A == B)
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
                        if (C == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }

                        if (delta <= Math.Abs(C) * rtol)
                            continue;
                        return false;
                    }

                }
                else if (atol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = b;
                        if (A == B)
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
                        var A = ptrA[i];
                        var B = b;
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                            continue;
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            return false;
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            return false;
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            return false;
                        if (A != B)
                            return false;
                    }

                }
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A != B)
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
        return IsEqual(b, a, rtol, atol);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        return IsEqual(b, a, rtol, atol);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool IsEqual(this Double a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        return IsEqual(b, a, rtol, atol);
    }

    /// <summary>
    ///   Determines whether two vectors contain the same values.
    /// </summary>
    /// 
    public static bool IsEqual(this Double a, Double b, Double atol = 0, Double rtol = 0)
    {
        if (rtol > 0)
        {
            {
                var A = a;
                var B = b;
                if (A == B)
                    return true;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    return true;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                var delta = Math.Abs(C - D);
                if (C == 0)
                {
                    if (delta <= rtol)
                        return true;
                }
                else if (D == 0)
                {
                    if (delta <= rtol)
                        return true;
                }

                if (delta <= Math.Abs(C) * rtol)
                    return true;
                return false;
            }

        }

        if (atol > 0)
        {
            {
                var A = a;
                var B = b;
                if (A == B)
                    return true;
                if (Double.IsNaN(A) && Double.IsNaN(B))
                    return true;
                if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    return false;
                if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    return false;
                if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    return false;
                var C = A;
                var D = B;
                if (Math.Round(Math.Abs(C - D), Tools.GetDecimalCount(atol)) <= atol)
                    return true;
                return false;
            }

        }
        {
            var A = a;
            var B = b;
            if (Double.IsNaN(A) && Double.IsNaN(B))
                return true;
            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                return false;
            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                return false;
            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                return false;
            if (A != B)
                return false;
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A == B)
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
                    if (C == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                            continue;
                    }

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
                    if (A == B)
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
                    if (A != B)
                        return false;
                }

        }

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
                if (C == 0)
                {
                    if (delta <= rtol)
                        continue;
                }
                else if (D == 0)
                {
                    if (delta <= rtol)
                        continue;
                }

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
}