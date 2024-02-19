using ISynergy.Framework.Mathematics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Random;
[TestClass]
public class ZigguratNormalGeneratorTest
{

    [TestMethod]
    public void TestZigguratNormalGenerator_Unseeded_InitializesCorrectly()
    {
        ZigguratNormalGenerator rng = new();
        double num = rng.Generate();

        Assert.IsNotNull(rng);
    }

    [TestMethod]
    public void TestZigguratNormalGenerator_WithSeed_InitializesCorrectly()
    {
        ZigguratNormalGenerator rng1 = new(seed: 457860009);
        ZigguratNormalGenerator rng2 = new(seed: 457860009);

        double num1 = rng1.Generate();
        double num2 = rng2.Generate();

        Console.WriteLine(num1);
        Assert.AreEqual(num1, num2);
    }

    [TestMethod]
    public void TestZigguratExponentialGenerator_Unseeded_InitializesCorrectly()
    {
        ZigguratExponentialGenerator rng = new();
        double num = rng.Generate();

        Assert.IsNotNull(rng);
    }

    [TestMethod]
    public void TestZigguratExponentialGenerator_Unseeded_NeverOverflows()
    {
        ZigguratExponentialGenerator rng = new();

        for (int i = 0; i < 100; i++)
        {
            double num = rng.Generate();
        }

        Assert.IsNotNull(rng);
    }

    [TestMethod]
    public void TestZigguratGenerator_Unseeded_NeverOverflows()
    {
        ZigguratNormalGenerator rng = new();

        for (int i = 0; i < 100; i++)
        {
            double num = rng.Generate();
        }

        Assert.IsNotNull(rng);
    }

    [TestMethod]
    public void TestZigguratExponentialGenerator_WithSeed_InitializesCorrectly()
    {
        ZigguratExponentialGenerator rng1 = new(seed: 457860009);
        ZigguratExponentialGenerator rng2 = new(seed: 457860009);

        double num1 = rng1.Generate();
        double num2 = rng2.Generate();

        Assert.AreEqual(num1, num2);
    }
}
