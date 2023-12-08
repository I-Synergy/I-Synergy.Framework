namespace ISynergy.Framework.Mathematics;

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

                if (A == B)
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

                if (A == B)
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
                    r[i] = (A == B);
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Double[,] a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        bool[,] r = MatrixCreateAs<Double, bool>(a);

        unsafe
        {
            fixed (Double* ptrA = a)
            fixed (Double* ptrB = b)
            fixed (bool* ptrR = r)
            {
                if (rtol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];

                        if (A == B)
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else
                        {
                            var C = A;
                            var D = B;
                            var delta = Math.Abs(C - D);

                            if (C == 0 && delta <= rtol)
                            {
                                ptrR[i] = true;
                            }
                            else if (D == 0 && delta <= rtol)
                            {
                                ptrR[i] = true;
                            }
                            else
                            {
                                ptrR[i] = (delta <= Math.Abs(C) * rtol);
                            }
                        }
                    }
                }
                else if (atol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];

                        if (A == B)
                        {
                            ptrR[i] = true; continue;
                        }
                        else if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true; continue;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else
                        {
                            ptrR[i] = (Math.Abs(A - B) <= atol);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];

                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true; continue;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false; continue;
                        }
                        else
                        {
                            ptrR[i] = (A == B); continue;
                        }
                    }
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double[,] a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        bool[][] r = JaggedCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];

                    if (A == B)
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
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];

                    if (A == B)
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
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                {
                    var A = a[i, j];
                    var B = b[i][j];

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
                        r[i][j] = (A == B);
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double[][] a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        bool[][] r = JaggedCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i, j];

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
                        r[i][j] = (A == B);
                    }
                }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double[][] a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        bool[][] r = JaggedCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b[i][j];

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
                        r[i][j] = (A == B);
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

                if (A == B)
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

                if (A == B)
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
                    r[i] = (A == B);
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Double[,] a, Double b, Double atol = 0, Double rtol = 0)
    {
        bool[,] r = MatrixCreateAs<Double, bool>(a);

        unsafe
        {
            fixed (Double* ptrA = a)
            fixed (bool* ptrR = r)
            {
                if (rtol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = b;

                        if (A == B)
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else
                        {
                            var C = A;
                            var D = B;
                            var delta = Math.Abs(C - D);

                            if (C == 0 && delta <= rtol)
                            {
                                ptrR[i] = true;
                            }
                            else if (D == 0 && delta <= rtol)
                            {
                                ptrR[i] = true;
                            }
                            else
                            {
                                ptrR[i] = (delta <= Math.Abs(C) * rtol);
                            }
                        }
                    }
                }
                else if (atol > 0)
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = b;

                        if (A == B)
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else
                        {
                            var C = A;
                            var D = B;

                            ptrR[i] = (Math.Abs(C - D) <= atol);
                        }
                    }

                }
                else
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = b;

                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        {
                            ptrR[i] = true;
                        }
                        else if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        {
                            ptrR[i] = false;
                        }
                        else
                        {
                            ptrR[i] = (A == B);
                        }
                    }
                }
            }
        }

        return r;
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double[][] a, Double b, Double atol = 0, Double rtol = 0)
    {
        bool[][] r = JaggedCreateAs<Double, bool>(a);

        if (rtol > 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b;

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
                    var B = b;

                    if (A == B)
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
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                {
                    var A = a[i][j];
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
                        r[i][j] = (A == B);
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
        return Equals(b, a, rtol, atol);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[,] Equals(Double a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        return Equals(b, a, rtol, atol);
    }

    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///
    public static bool[][] Equals(Double a, Double[][] b, Double atol = 0, Double rtol = 0)
    {
        return Equals(b, a, rtol, atol);
    }
}