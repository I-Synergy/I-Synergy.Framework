namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    public partial class MatrixTest
    {

        [TestMethod]
        public void create_nullable_test()
        {
            double?[] ones = Vector.Ones<double?>(5);
            CollectionAssert.AreEqual(new double?[] { 1, 1, 1, 1, 1 }, ones);
        }

        [TestMethod]
        public void CreateJaggedTest()
        {
            Array jagged = Jagged.Zeros(typeof(int), 2, 3, 1);

            foreach (var idx in jagged.GetIndices(deep: true))
            {
                Assert.AreEqual(0, jagged.GetValue(deep: true, indices: idx));
                jagged.SetValue(idx.Sum(), deep: true, indices: idx);
            }

            int[][][] expected =
            {
                new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2 } },
                new int[][] { new[] { 1 }, new[] { 2 }, new[] { 3 } }
            };

            Assert.IsTrue(expected.IsEqual(jagged));
        }

        [TestMethod]
        public void CreateMatrixTest()
        {
            Array matrix = Matrix.Zeros(typeof(int), 2, 3, 1);

            foreach (var idx in matrix.GetIndices())
            {
                Assert.AreEqual(0, matrix.GetValue(deep: true, indices: idx));
                matrix.SetValue(idx.Sum(), deep: true, indices: idx);
            }

            int[,,] expected =
            {
                { { 0 }, { 1 }, { 2 } },
                { { 1 }, { 2 }, { 3 } }
            };

            Assert.IsTrue(expected.IsEqual(matrix));
        }

        [TestMethod]
        public void EnumerateJaggedTest()
        {
            int[][][] input =
            {
                new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2 } },
                new int[][] { new[] { 1 }, new[] { 2 }, new[] { 3 } }
            };

            int[] expected = { 0, 1, 2, 1, 2, 3 };

            List<int> actual = new List<int>();
            foreach (object obj in Jagged.Enumerate(input, new int[] { 2, 3, 1 }))
                actual.Add((int)obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));

            actual.Clear();
            foreach (int obj in Jagged.Enumerate<int>(input, new int[] { 2, 3, 1 }))
                actual.Add(obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));
        }

        [TestMethod]
        public void EnumerateJaggedTest2()
        {
            int[][] input =
            {
                new int[] { 0, 1, 2 },
                new int[] { 1, 2, 3 }
            };

            int[] expected = { 0, 1, 2, 1, 2, 3 };

            List<int> actual = new List<int>();
            foreach (object obj in Jagged.Enumerate(input, new int[] { 2 }))
                actual.AddRange((int[])obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));

            actual.Clear();
            foreach (int[] obj in Jagged.Enumerate<int[]>(input, new int[] { 2 }))
                actual.AddRange(obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));
        }

        [TestMethod]
        public void EnumerateJaggedNoShapeTest()
        {
            int[][][] input =
            {
                new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2 } },
                new int[][] { new[] { 1 }, new[] { 2 }, new[] { 3 } }
            };

            int[] expected = { 0, 1, 2, 1, 2, 3 };

            List<int> actual = new List<int>();
            foreach (object obj in Jagged.Enumerate(input))
                actual.Add((int)obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));

            actual.Clear();
            foreach (int obj in Jagged.Enumerate<int>(input))
                actual.Add(obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));
        }

        [TestMethod]
        public void EnumerateJaggedVariableLengthTest()
        {
            int[][][] input =
            {
                new int[][] { new[] { 0 }, new[] { 1 } },
                new int[][] { new[] { 9 }, new int[] { }, new[] { 3 } }
            };

            int[] expected = { 0, 1, 0, 9, 0, 3 };

            List<int> actual = new List<int>();
            foreach (object obj in Jagged.Enumerate(input, new int[] { 2, 3, 1 }))
                actual.Add(obj == null ? 0 : (int)obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));

            actual.Clear();
            foreach (int obj in Jagged.Enumerate<int>(input, new int[] { 2, 3, 1 }))
                actual.Add(obj);

            Assert.IsTrue(expected.IsEqual(actual.ToArray()));
        }


        [TestMethod]
        public void StackTest()
        {
            var x1 = Vector.Ones(1000);
            var y1 = Vector.Zeros(1000);

            double[,] w1 = Matrix.Stack(x1, y1).Transpose();

            Assert.AreEqual(1000, w1.Rows());
            Assert.AreEqual(2, w1.Columns());
            Assert.AreEqual(w1.Length, x1.Length + y1.Length);

            for (int i = 0; i < x1.Length; i++)
            {
                Assert.AreEqual(1, x1[i]);
                Assert.AreEqual(1, w1[i, 0]);
            }

            for (int i = 0; i < y1.Length; i++)
            {
                Assert.AreEqual(0, y1[i]);
                Assert.AreEqual(0, w1[i, 1]);
            }

            var x = w1.GetColumn(0);
            var y = w1.GetColumn(1);

            Assert.IsTrue(x.IsEqual(x1));
            Assert.IsTrue(y.IsEqual(y1));
        }
    }
}
