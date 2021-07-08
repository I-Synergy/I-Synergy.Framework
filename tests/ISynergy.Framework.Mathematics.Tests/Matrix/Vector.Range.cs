﻿namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Core.Extensions;
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class VectorTest
    {

        [TestMethod]
        public void gh898()
        {
            // https://github.com/accord-net/framework/issues/898
            double[] vec1 = Vector.Range(0d, 5d, 1d);                       // { 0, 1, 2, 3, 5 }
            double[] vec2 = Vector.EnumerableRange(0d, 5d).ToArray();       // { 0, 1, 2, 3, 4 }
            double[] vec3 = Vector.EnumerableRange(0d, 5d, 1d).ToArray();   // { 0, 1, 2, 3, 4, 5 }

            CollectionAssert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec1);
            CollectionAssert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec2);
            CollectionAssert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec3);
        }

        [TestMethod]
        public void range_test()
        {
            // Sanity checks
            CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, Vector.Range(3));
            CollectionAssert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1));
            CollectionAssert.AreEqual(new int[] { 2, 1 }, Vector.Range(2, 0));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, 1));
            CollectionAssert.AreEqual(new int[] { 2, 1 }, Vector.Range(2, 0, 1));

            // Deep tests
            for (double end = 0; end < 10; end++)
            {
                test(0, end, () => Vector.Range(end));
                //test(0, end, () => Vector.Range((short)end));
                //test(0, end, () => Vector.Range((byte)end));
                //test(0, end, () => Vector.Range((double)end));
                //test(0, end, () => Vector.Range((float)end));
                //test(0, end, () => Vector.Range((long)end));
                //test(0, end, () => Vector.Range((ulong)end));
                //test(0, end, () => Vector.Range((ushort)end));

                for (double start = 0; start < 10; start++)
                {
                    test(start, end, () => Vector.Range(start, end, stepSize: 1));
                    //test(start, end, () => Vector.Range((short)start, (short)end, (short)1));
                    //test(start, end, () => Vector.Range((byte)start, (byte)end, stepSize: (byte)1));
                    //test(start, end, () => Vector.Range((double)start, (double)end, (double)1));
                    //test(start, end, () => Vector.Range((float)start, (float)end, (float)1));
                    //test(start, end, () => Vector.Range((long)start, (long)end, (long)1));
                    //test(start, end, () => Vector.Range((ulong)start, (ulong)end, (ulong)1));
                    //test(start, end, () => Vector.Range((ushort)start, (ushort)end, (ushort)1));
                }
            }
        }

        [TestMethod]
        public void enumerable_range_test()
        {
            // Sanity checks
            CollectionAssert.AreEqual(new double[] { 0, 1, 2 }, Vector.EnumerableRange(3).ToArray());
            CollectionAssert.AreEqual(new double[] { 1, 2 }, Vector.EnumerableRange(1, 3).ToArray());
            CollectionAssert.AreEqual(new double[] { 3, 2 }, Vector.EnumerableRange(3, 1).ToArray());
            CollectionAssert.AreEqual(new double[] { 2, 1 }, Vector.EnumerableRange(2, 0).ToArray());
            CollectionAssert.AreEqual(new double[] { 3, 2 }, Vector.EnumerableRange(3, 1, 1).ToArray());
            CollectionAssert.AreEqual(new double[] { 2, 1 }, Vector.EnumerableRange(2, 0, 1).ToArray());

            // Deep tests
            for (double end = 0; end < 10; end++)
            {
                test(0, end, () => Vector.EnumerableRange(end));

                for (double start = 0; start < 10; start++)
                {
                    test(start, end, () => Vector.EnumerableRange(start, end, stepSize: 1));
                }
            }
        }

        [TestMethod]
        public void numpy_comparison_tests()
        {
            CollectionAssert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3, +1));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, -1));
            CollectionAssert.AreEqual(new int[] { 1 }, Vector.Range(1, 3, +5));
            CollectionAssert.AreEqual(new int[] { 3 }, Vector.Range(3, 1, -5));

            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.Range(0, 1, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.Range(1, 0, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.Range(0, 1, 0.2), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.Range(1, 0, -0.2), 1e-10));

            // The framework's version differs when the intervals are inverted
            // and the step is positive. In this case, the framework will still
            // iterate over the range backwards because the third parameter is 
            // considered a step <i>size</i>, instead of a step direction.
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, +1));

            // However, it is not allowed to specify a negative step size when
            // a < b is not allowed since it would result in an infinite loop:
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Vector.Range(1, 3, -1));
        }

        [TestMethod]
        public void range_step_test()
        {
            // Sanity checks
            CollectionAssert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3, +1));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, -1));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, +1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Vector.Range(1, 3, -1));
            CollectionAssert.AreEqual(new int[] { 1 }, Vector.Range(1, 3, +5));
            CollectionAssert.AreEqual(new int[] { 3 }, Vector.Range(3, 1, -5));
            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.Range(0, 1, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.Range(1, 0, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.Range(0, 1, 0.2), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.Range(1, 0, -0.2), 1e-10));

            // Deep tests
            for (double step = -2; step < 2; step += 0.1)
            {
                if (Math.Abs(step) < 1e-10)
                    step = 0;

                for (double end = 0; end < 5; end++)
                {
                    for (double start = 0; start < 5; start++)
                    {
                        test(start, end, step, () => Vector.Range(start, end, step));
                    }
                }
            }
        }

        [TestMethod]
        public void enumerable_range_step_test()
        {
            // Sanity checks
            CollectionAssert.AreEqual(new double[] { 1, 2 }, Vector.EnumerableRange(1, 3, +1).ToArray());
            CollectionAssert.AreEqual(new double[] { 3, 2 }, Vector.EnumerableRange(3, 1, -1).ToArray());
            CollectionAssert.AreEqual(new double[] { 3, 2 }, Vector.EnumerableRange(3, 1, +1).ToArray());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Vector.EnumerableRange(1, 3, -1).ToArray());
            CollectionAssert.AreEqual(new double[] { 1 }, Vector.EnumerableRange(1, 3, +5).ToArray());
            CollectionAssert.AreEqual(new double[] { 3 }, Vector.EnumerableRange(3, 1, -5).ToArray());
            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.EnumerableRange(0, 1, 0.3).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.EnumerableRange(1, 0, 0.3).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.EnumerableRange(0, 1, 0.2).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.EnumerableRange(1, 0, -0.2).ToArray(), 1e-10));

            // Deep tests
            for (double step = -2; step < 2; step += 0.1)
            {
                if (Math.Abs(step) < 1e-10)
                    step = 0;

                for (double end = 0; end < 5; end++)
                {
                    for (double start = 0; start < 5; start++)
                    {
                        test(start, end, step, () => Vector.EnumerableRange(start, end, step));
                    }
                }
            }
        }




        private static void test<T>(double start, double end, Func<IEnumerable<T>> func)
        {
            test(start, end, () => func().ToArray());
        }

        private static void test<T>(double start, double end, double step, Func<IEnumerable<T>> func)
        {
            test(start, end, step, () => func().ToArray());
        }

        private static void test<T>(double start, double end, Func<T[]> func)
        {
            T[] values = func();

            if (start == end)
            {
                Assert.AreEqual(0, values.Length);
            }
            else if (start < end)
            {
                Assert.AreEqual(start, values.Get(0));
                Assert.AreEqual(end - 1, values.Get(-1));
            }
            else
            {
                Assert.AreEqual(start, values.Get(0));
                Assert.AreEqual(end + 1, values.Get(-1));
            }
        }

        private static void test<T>(double start, double end, double step, Func<T[]> func)
        {
            double tol = 1e-10;
            if (typeof(T) == typeof(float))
                tol = 1e-6;

            if (start == end)
            {
                T[] values = func();
                Assert.AreEqual(0, values.Length);
            }
            else
            {
                if (step == 0)
                {
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => func());
                }
                else
                {
                    if (start < end)
                    {
                        if (step > 0)
                        {
                            T[] values = func();
                            Assert.AreEqual(start, values.Get(0));
                            Assert.AreEqual(start + (values.Length - 1) * step, values.Get(-1).To<double>(), tol);
                        }
                        else
                        {
                            Assert.ThrowsException<ArgumentOutOfRangeException>(() => func());
                        }
                    }
                    else
                    {
                        T[] values = func();
                        Assert.AreEqual(start, values.Get(0));
                        if ((start - end) <= step)
                            Assert.AreEqual(start - (values.Length - 1) * step, values.Get(-1).To<double>(), tol);
                    }
                }
            }
        }

    }
}
