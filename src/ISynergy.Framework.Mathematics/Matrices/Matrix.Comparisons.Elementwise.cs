using System.Runtime.InteropServices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

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

        if (a.Length == 0)
            return r;

        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        var spanB = MemoryMarshal.CreateSpan(ref b[0, 0], b.Length);
        var spanR = MemoryMarshal.CreateSpan(ref r[0, 0], r.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = spanB[i];

                if (A == B)
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

                if (A == B)
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
                spanR[i] = (A == B); continue;
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

        if (a.Length == 0)
            return r;

        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        var spanR = MemoryMarshal.CreateSpan(ref r[0, 0], r.Length);
        if (rtol > 0)
        {
            for (var i = 0; i < spanA.Length; i++)
            {
                var A = spanA[i];
                var B = b;

                if (A == B)
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

                if (A == B)
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
                    spanR[i] = (A == B);
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