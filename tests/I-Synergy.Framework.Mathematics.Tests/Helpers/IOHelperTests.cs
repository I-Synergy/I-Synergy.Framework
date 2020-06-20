using System;
using System.Globalization;
using System.IO;
using Xunit;

namespace ISynergy.Framework.Mathematics.Tests
{
    /// <summary>
    /// Class IOHelperTests.
    /// </summary>
    public class IOHelperTests
    {
        /// <summary>
        /// The x
        /// </summary>
        private double[] X = Array.Empty<double>();
        /// <summary>
        /// The source mx
        /// </summary>
        private string sourceMx = string.Empty;
        /// <summary>
        /// The source out
        /// </summary>
        private string sourceOut = string.Empty;
        /// <summary>
        /// From
        /// </summary>
        const int from = 1;

        /// <summary>
        /// The decomposition
        /// </summary>
        private readonly LUDecomposition decomposition = new LUDecomposition();

        /// <summary>
        /// Reads the array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="num">The number.</param>
        /// <returns>System.Double[].</returns>
        private double[] ReadArray(string source, int num)
        {
            var XX = new double[num];
            var i = 0;

            using (var fs = new StreamReader(source))
            {
                var line = fs.ReadLine();

                if(!string.IsNullOrEmpty(line))
                {
                    var parts = line.Split(' ');

                    foreach (var part in parts)
                    {
                        double.TryParse(part, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out XX[i]);
                        i++;
                    }
                }
            }

            return XX;
        }

        /// <summary>
        /// Defines the test method TestReadFile_4.
        /// </summary>
        [Fact]
        public void TestReadFile_4()
        {
            sourceMx = @"Assets\Files\Matrix4.txt";

            var XX = new double[] { 0.73421, 0.27065, -1.05448, -0.50694 };

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, XX));
        }

        /// <summary>
        /// Defines the test method TestReadFile_10.
        /// </summary>
        [Fact]
        public void TestReadFile_10()
        {
            sourceMx = @"Assets\Files\Matrix10.txt";
            sourceOut = @"Assets\Files\Array10.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 10)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_10r.
        /// </summary>
        [Fact]
        public void TestReadFile_10r()
        {
            sourceMx = @"Assets\Files\Matrix10r.txt";
            sourceOut = @"Assets\Files\Array10r.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 10)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_15r.
        /// </summary>
        [Fact]
        public void TestReadFile_15r()
        {
            sourceMx = @"Assets\Files\Matrix15r.txt";
            sourceOut = @"Assets\Files\Array15r.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 15)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_20r.
        /// </summary>
        [Fact]
        public void TestReadFile_20r()
        {
            sourceMx = @"Assets\Files\Matrix20r.txt";
            sourceOut = @"Assets\Files\Array20.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 20)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_25r.
        /// </summary>
        [Fact]
        public void TestReadFile_25r()
        {
            sourceMx = @"Assets\Files\Matrix25r.txt";
            sourceOut = @"Assets\Files\Array25.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 25)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_30r.
        /// </summary>
        [Fact]
        public void TestReadFile_30r()
        {
            sourceMx = @"Assets\Files\Matrix30r.txt";
            sourceOut = @"Assets\Files\Array30.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 30)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_35r.
        /// </summary>
        [Fact]
        public void TestReadFile_35r()
        {
            sourceMx = @"Assets\Files\Matrix35r.txt";
            sourceOut = @"Assets\Files\Array35.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 35)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_40r.
        /// </summary>
        [Fact]
        public void TestReadFile_40r()
        {
            sourceMx = @"Assets\Files\Matrix40r.txt";
            sourceOut = @"Assets\Files\Array40r.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 40)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_45r.
        /// </summary>
        [Fact]
        public void TestReadFile_45r()
        {
            sourceMx = @"Assets\Files\Matrix45r.txt";
            sourceOut = @"Assets\Files\Array45.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 45)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_50r.
        /// </summary>
        [Fact]
        public void TestReadFile_50r()
        {
            sourceMx = @"Assets\Files\Matrix50r.txt";
            sourceOut = @"Assets\Files\Array50.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 50)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_40.
        /// </summary>
        [Fact]
        public void TestReadFile_40()
        {
            sourceMx = @"Assets\Files\Matrix40.txt";
            sourceOut = @"Assets\Files\Array40.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 40)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_100r.
        /// </summary>
        [Fact]
        public void TestReadFile_100r()
        {
            sourceMx = @"Assets\Files\Matrix100r.txt";
            sourceOut = @"Assets\Files\Array100.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 100)));
        }

        /// <summary>
        /// Defines the test method TestReadFile_200r.
        /// </summary>
        [Fact]
        public void TestReadFile_200r()
        {
            sourceMx = @"Assets\Files\Matrix200r.txt";
            sourceOut = @"Assets\Files\Array200.txt";

            decomposition.Calculate_LUDecomposition(sourceMx, from, out X);

            Assert.True(CheckValues(X, ReadArray(sourceOut, 200)));
        }

        /// <summary>
        /// Checks the values.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckValues(double[] x, double[] values)
        {
            var result = false;
            var i = 0;

            while (!result && i < x.Length)
            {
                if (Math.Round(x[i], 5) == values[i])
                {
                    result = true;
                }
                else
                {
                    return false;
                }

                i++;
            }

            return result;
        }
    }
}
