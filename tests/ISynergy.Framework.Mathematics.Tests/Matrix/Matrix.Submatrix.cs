using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{
    public partial class MatrixTest
    {

        
        [TestMethod]
        public void MatrixSubmatrix()
        {
            double[,] value = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 2.000, 2.000, 2.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] actual = Matrix.Get(value, new int[] { 0, 2 });

            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }

        [TestMethod]
        public void MatrixSubmatrix2()
        {
            double[,] value = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 2.000, 2.000, 2.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] actual;

            actual = Matrix.Get(value, new int[] { 0, 2 });
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Get(value, new int[] { 0, 2 }, null);
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Submatrix(value, null, null);
            Assert.IsTrue(Matrix.IsEqual(actual, value));
        }

        [TestMethod]
        public void SubmatrixTest()
        {
            double[][] data = 
            {
                new double[] { 1, 2, 3 },
                new double[] { 4, 5, 6 },
                new double[] { 7, 8, 9 },
            };

            int[] rowIndexes = { 1, 2 };
            int j0 = 0;
            int j1 = 1;

            double[][] expected = 
            {
                //new double[] { 1, 2, 3 },
                new double[] { 4, 5/*, 6*/ },
                new double[] { 7, 8/*, 9*/ },
            };

            double[][] actual = Matrix.Get(data, rowIndexes, j0, j1);

            for (int i = 0; i < actual.GetLength()[0]; i++)
                for (int j = 0; j < actual.GetLength()[1]; j++)
                    Assert.AreEqual(expected[i][j], actual[i][j]);

            double[][] expected2 = 
            {
                new double[] { 1, 2/*, 3*/ },
                new double[] { 4, 5/*, 6*/ },
                new double[] { 7, 8/*, 9*/ },
            };

            double[][] actual2 = Matrix.Get(data, null, j0, j1);

            for (int i = 0; i < actual2.GetLength()[0]; i++)
                for (int j = 0; j < actual2.GetLength()[1]; j++)
                    Assert.AreEqual(expected2[i][j], actual2[i][j]);
        }

        [TestMethod]
        public void SubmatrixTest1()
        {
            double[,] value = new double[,]
            { 
                { 1.000, 1.000, 1.000 },
                { 2.000, 2.000, 2.000 },
                { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
                {        1.000, 1.000 },
                {        3.000, 3.000 }
            };

            int[] rowIndexes = { 0, 2 };
            int j0 = 1;
            int j1 = 2;

            double[,] actual;

            actual = Matrix.Get(value, rowIndexes, j0, j1 + 1);
            Assert.IsTrue(Matrix.IsEqual(actual, expected));


            double[,] expected2 = new double[,]
            { 
                {        1.000, 1.000 },
                {        2.000, 2.000 },
                {        3.000, 3.000 }
            };

            actual = Matrix.Get(value, null, j0, j1 + 1);
            Assert.IsTrue(Matrix.IsEqual(actual, expected2));

            actual = Matrix.Submatrix(value, null, null);
            Assert.IsTrue(Matrix.IsEqual(actual, value));
        }

        [TestMethod]
        public void SubgroupTest2()
        {
            double[] value = { 1, 2, 3, 4, 5, 6, 7 };
            int[] idx = { 0, 0, 0, 5, 5, 5, 5 };


            double[][] groups = value.Subgroups(idx);

            Assert.AreEqual(2, groups.Length);
            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(4, groups[1].Length);

            Assert.AreEqual(groups[0][0], 1);
            Assert.AreEqual(groups[0][1], 2);
            Assert.AreEqual(groups[0][2], 3);

            Assert.AreEqual(groups[1][0], 4);
            Assert.AreEqual(groups[1][1], 5);
            Assert.AreEqual(groups[1][2], 6);
            Assert.AreEqual(groups[1][3], 7);
        }

        [TestMethod]
        public void SubgroupTest3()
        {
            double[] value = { 1, 2, 3, 4, 5, 6, 7 };
            int[] idx = { 0, 0, 0, 4, 4, 4, 4 };


            double[][] groups = value.Subgroups(idx, 5);

            Assert.AreEqual(5, groups.Length);

            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(0, groups[1].Length);
            Assert.AreEqual(0, groups[2].Length);
            Assert.AreEqual(0, groups[3].Length);
            Assert.AreEqual(4, groups[4].Length);

            Assert.AreEqual(groups[0][0], 1);
            Assert.AreEqual(groups[0][1], 2);
            Assert.AreEqual(groups[0][2], 3);

            Assert.AreEqual(groups[4][0], 4);
            Assert.AreEqual(groups[4][1], 5);
            Assert.AreEqual(groups[4][2], 6);
            Assert.AreEqual(groups[4][3], 7);
        }
    }
}
