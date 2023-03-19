namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Integration;
    using ISynergy.Framework.Mathematics.Random;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class MonteCarloIntegralTest
    {

        [TestMethod]
        public void MonteCarloTest()
        {
            Generator.Seed = 0;

            // A common Monte-Carlo integration example is to compute
            // the value of Pi. This is the same example given in the
            // Wikipedia's page for Monte-Carlo Integration at
            // https://en.wikipedia.org/wiki/Monte_Carlo_integration#Example

            Func<double, double, double> H =
                (x, y) => (x * x + y * y <= 1) ? 1 : 0;

            double[] from = { -1, -1 };
            double[] to = { +1, +1 };

            int samples = 1000000;

            double area = MonteCarloIntegration.Integrate(x => H(x[0], x[1]), from, to, samples);

            Assert.AreEqual(Math.PI, area, 5e-3);
        }


    }
}
