namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Random;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class ZigguratNormalGeneratorTest
    {

        [TestMethod]
        public void TestZigguratNormalGenerator_Unseeded_InitializesCorrectly()
        {
            var rng = new ZigguratNormalGenerator();
            double num = rng.Generate();

            Assert.IsNotNull(rng);
        }

        [TestMethod]
        public void TestZigguratNormalGenerator_WithSeed_InitializesCorrectly()
        {
            var rng1 = new ZigguratNormalGenerator(seed: 457860009);
            var rng2 = new ZigguratNormalGenerator(seed: 457860009);

            double num1 = rng1.Generate();
            double num2 = rng2.Generate();

            Console.WriteLine(num1);
            Assert.AreEqual(num1, num2);
        }

        [TestMethod]
        public void TestZigguratExponentialGenerator_Unseeded_InitializesCorrectly()
        {
            var rng = new ZigguratExponentialGenerator();
            double num = rng.Generate();

            Assert.IsNotNull(rng);
        }

        [TestMethod]
        public void TestZigguratExponentialGenerator_Unseeded_NeverOverflows()
        {
            var rng = new ZigguratExponentialGenerator();

            for (int i = 0; i < 100; i++)
            {
                double num = rng.Generate();
            }

            Assert.IsNotNull(rng);
        }

        [TestMethod]
        public void TestZigguratGenerator_Unseeded_NeverOverflows()
        {
            var rng = new ZigguratNormalGenerator();

            for (int i = 0; i < 100; i++)
            {
                double num = rng.Generate();
            }

            Assert.IsNotNull(rng);
        }

        [TestMethod]
        public void TestZigguratExponentialGenerator_WithSeed_InitializesCorrectly()
        {
            var rng1 = new ZigguratExponentialGenerator(seed: 457860009);
            var rng2 = new ZigguratExponentialGenerator(seed: 457860009);

            double num1 = rng1.Generate();
            double num2 = rng2.Generate();

            Assert.AreEqual(num1, num2);
        }
    }
}
