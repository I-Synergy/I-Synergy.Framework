using ISynergy.Framework.Mathematics.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{
    /// <summary>
    /// Class MatrixMultiplicationTests.
    /// </summary>
    [TestClass]
    public class MatrixMultiplicationTests
    {
        /// <summary>
        /// Defines the test method NormalMultiplyWithMatrix.
        /// </summary>
        [TestMethod]
        public void NormalMultiplyWithMatrix()
        {
            var a = new Matrix(new double[,]
                {
                    {1,2,0},
                    {-1,2,3},
                    {0,1,1}
                });

            var b = new Matrix(new double[,]
                {
                    {1,2},
                    {0,1},
                    {-1,0}
                });

            var expected = new Matrix(new double[,]
                {
                    {1,4},
                    {-4,0},
                    {-1,1}
                });

            var actual = Matrix.NormalMultiply(a, b);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Defines the test method NormalMultiplyWithIdentityMatrix.
        /// </summary>
        [TestMethod]
        public void NormalMultiplyWithIdentityMatrix()
        {
            var a = new Matrix(new double[,]
                {
                    {1,2,3},
                    {4,5,6},
                    {7,8,9}
                });

            var b = MatrixGenerator.IdentityMatrix(3);

            var expected = new Matrix(new double[,]
                {
                    {1,2,3},
                    {4,5,6},
                    {7,8,9}
                });

            var actual = Matrix.NormalMultiply(a, b);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Defines the test method StrassenMultiplyWithMatrix1.
        /// </summary>
        [TestMethod]
        public void StrassenMultiplyWithMatrix1()
        {
            var a = new Matrix(new double[,]
                {
                    {1,2},
                    {1,2}
                });

            var b = new Matrix(new double[,]
                {
                    {1,2},
                    {1,2}
                });

            var expected = new Matrix(new double[,]
                {
                    {3,6},
                    {3,6}
                });

            var actual = Matrix.StrassenMultiply(a, b);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Defines the test method StrassenMultiplyWithMatrix2.
        /// </summary>
        [TestMethod]
        public void StrassenMultiplyWithMatrix2()
        {
            var a = new Matrix(new double[,]
                {
                    {1,2,3,4},
                    {1,2,3,4},
                    {1,2,3,4},
                    {1,2,3,4}
                });

            var b = new Matrix(new double[,]
                {
                    {1,2,3,4},
                    {1,2,3,4},
                    {1,2,3,4},
                    {1,2,3,4}
                });

            var expected = new Matrix(new double[,]
                {
                    {10,20,30,40},
                    {10,20,30,40},
                    {10,20,30,40},
                    {10,20,30,40}
                });

            var actual = Matrix.StrassenMultiply(a, b);

            Assert.AreEqual(expected, actual);
        }
    }
}
