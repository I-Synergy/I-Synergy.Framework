namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Convergence;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RelativeParameterConvergenceTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        public void RelativeParameterConstructorTest()
        {
            var criteria = new RelativeParameterConvergence(iterations: 0, tolerance: 0.1);

            int progress = 1;
            double[] parameters = { 12345.6, 952.12, 1925.1 };

            do
            {
                // Do some processing...


                // Update current iteration information:
                criteria.NewValues = parameters.Divide(progress++);

            } while (!criteria.HasConverged);


            // The method will converge after reaching the 
            // maximum of 11 iterations with a final value
            // of { 1234.56, 95.212, 192.51 }:

            int iterations = criteria.CurrentIteration; // 11
            var v = criteria.OldValues; // { 1234.56, 95.212, 192.51 }


            Assert.AreEqual(11, criteria.CurrentIteration);
            Assert.AreEqual(1234.56, criteria.OldValues[0]);
            Assert.AreEqual(95.212, criteria.OldValues[1]);
            Assert.AreEqual(192.51, criteria.OldValues[2]);
        }
    }
}
