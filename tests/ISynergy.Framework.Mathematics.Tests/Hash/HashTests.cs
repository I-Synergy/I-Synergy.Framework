using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{
    /// <summary>
    /// Class HashTests.
    /// </summary>
    [TestClass]
    public class HashTests
    {
        /// <summary>
        /// The hash
        /// </summary>
        readonly Hash hash = new Hash();
        /// <summary>
        /// The test array
        /// </summary>
        readonly double[] testArray = new double[] { 3.32, 0.10, 0.00, 7.89, 0.00, 0.00 };

        /// <summary>
        /// Defines the test method TestHashGetSet.
        /// </summary>
        [TestMethod]
        public void TestHashGetSet()
        {
            var i = 0;

            foreach (var x in testArray)
            {
                hash.SetValue(i, x);
                i++;
            }

            var X = new double[testArray.Length];

            for (var j = 0; j < i; j++)
            {
                X[j] = Math.Round(hash.GetValue(j), 2);
            }

            Assert.AreEqual(testArray[0], X[0]);
            Assert.AreEqual(testArray[1], X[1]);
            Assert.AreEqual(testArray[2], X[2]);
            Assert.AreEqual(testArray[3], X[3]);
            Assert.AreEqual(testArray[4], X[4]);
            Assert.AreEqual(testArray[5], X[5]);
        }

        /// <summary>
        /// Defines the test method TestHashAdd.
        /// </summary>
        [TestMethod]
        public void TestHashAdd()
        {
            var testNewArray = new double[] { 9.32, 6.10, 6.00, 13.89, 6.00, 6.00 };
            var i = 0;

            foreach (var x in testArray)
            {
                hash.SetValue(i, x);
                i++;
            }

            var X = new double[testArray.Length];

            for (var j = 0; j < i; j++)
            {
                X[j] = Math.Round(hash.GetValue(j), 2) + 6;
            }

            Assert.AreEqual(testNewArray[0], X[0]);
            Assert.AreEqual(testNewArray[1], X[1]);
            Assert.AreEqual(testNewArray[2], X[2]);
            Assert.AreEqual(testNewArray[3], X[3]);
            Assert.AreEqual(testNewArray[4], X[4]);
            Assert.AreEqual(testNewArray[5], X[5]);
        }
    }
}
