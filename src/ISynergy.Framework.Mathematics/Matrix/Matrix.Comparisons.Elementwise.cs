using System;

namespace ISynergy.Framework.Mathematics
{
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
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if (C == 0)
                    {
                        if (delta <= rtol)
                        { r[i] = true; continue; }
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                        { r[i] = true; continue; }
                    }

                    { r[i] = (delta <= Math.Abs(C) * rtol); continue; }
                }

            }
            else if (atol > 0)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    var A = a[i];
                    var B = b[i];
                    if (A == B)
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    var C = A;
                    var D = B;
                    { r[i] = (Math.Abs(C - D) <= atol); continue; }
                }

            }
            else
            {
                for (var i = 0; i < a.Length; i++)
                {
                    var A = a[i];
                    var B = b[i];
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    { r[i] = (A == B); continue; }
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
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            var C = A;
                            var D = B;
                            var delta = Math.Abs(C - D);
                            if (C == 0)
                            {
                                if (delta <= rtol)
                                { ptrR[i] = true; continue; }
                            }
                            else if (D == 0)
                            {
                                if (delta <= rtol)
                                { ptrR[i] = true; continue; }
                            }

                            { ptrR[i] = (delta <= Math.Abs(C) * rtol); continue; }
                        }

                    }
                    else if (atol > 0)
                    {
                        for (var i = 0; i < a.Length; i++)
                        {
                            var A = ptrA[i];
                            var B = ptrB[i];
                            if (A == B)
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            var C = A;
                            var D = B;
                            { ptrR[i] = (Math.Abs(C - D) <= atol); continue; }
                        }

                    }
                    else
                    {
                        for (var i = 0; i < a.Length; i++)
                        {
                            var A = ptrA[i];
                            var B = ptrB[i];
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            { ptrR[i] = (A == B); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);
                        if (C == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }

                        { r[i][j] = (delta <= Math.Abs(C) * rtol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        { r[i][j] = (Math.Abs(C - D) <= atol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        { r[i][j] = (A == B); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);
                        if (C == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }

                        { r[i][j] = (delta <= Math.Abs(C) * rtol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        { r[i][j] = (Math.Abs(C - D) <= atol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        { r[i][j] = (A == B); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);
                        if (C == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }

                        { r[i][j] = (delta <= Math.Abs(C) * rtol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        { r[i][j] = (Math.Abs(C - D) <= atol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        { r[i][j] = (A == B); continue; }
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
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    var C = A;
                    var D = B;
                    var delta = Math.Abs(C - D);
                    if (C == 0)
                    {
                        if (delta <= rtol)
                        { r[i] = true; continue; }
                    }
                    else if (D == 0)
                    {
                        if (delta <= rtol)
                        { r[i] = true; continue; }
                    }

                    { r[i] = (delta <= Math.Abs(C) * rtol); continue; }
                }

            }
            else if (atol > 0)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    var A = a[i];
                    var B = b;
                    if (A == B)
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    var C = A;
                    var D = B;
                    { r[i] = (Math.Abs(C - D) <= atol); continue; }
                }

            }
            else
            {
                for (var i = 0; i < a.Length; i++)
                {
                    var A = a[i];
                    var B = b;
                    if (Double.IsNaN(A) && Double.IsNaN(B))
                    { r[i] = true; continue; }
                    if (Double.IsNaN(A) ^ Double.IsNaN(B))
                    { r[i] = false; continue; }
                    if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                    { r[i] = false; continue; }
                    if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                    { r[i] = false; continue; }
                    { r[i] = (A == B); continue; }
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
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            var C = A;
                            var D = B;
                            var delta = Math.Abs(C - D);
                            if (C == 0)
                            {
                                if (delta <= rtol)
                                { ptrR[i] = true; continue; }
                            }
                            else if (D == 0)
                            {
                                if (delta <= rtol)
                                { ptrR[i] = true; continue; }
                            }

                            { ptrR[i] = (delta <= Math.Abs(C) * rtol); continue; }
                        }

                    }
                    else if (atol > 0)
                    {
                        for (var i = 0; i < a.Length; i++)
                        {
                            var A = ptrA[i];
                            var B = b;
                            if (A == B)
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            var C = A;
                            var D = B;
                            { ptrR[i] = (Math.Abs(C - D) <= atol); continue; }
                        }

                    }
                    else
                    {
                        for (var i = 0; i < a.Length; i++)
                        {
                            var A = ptrA[i];
                            var B = b;
                            if (Double.IsNaN(A) && Double.IsNaN(B))
                            { ptrR[i] = true; continue; }
                            if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            { ptrR[i] = false; continue; }
                            if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            { ptrR[i] = false; continue; }
                            { ptrR[i] = (A == B); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);
                        if (C == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                            { r[i][j] = true; continue; }
                        }

                        { r[i][j] = (delta <= Math.Abs(C) * rtol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        var C = A;
                        var D = B;
                        { r[i][j] = (Math.Abs(C - D) <= atol); continue; }
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
                        { r[i][j] = true; continue; }
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                        { r[i][j] = false; continue; }
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                        { r[i][j] = false; continue; }
                        { r[i][j] = (A == B); continue; }
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
}