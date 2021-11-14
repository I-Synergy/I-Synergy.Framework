namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Convergence;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class AbsoluteConvergenceTest
    {

        [TestMethod]
        public void AbsoluteConvergenceConstructorTest()
        {
            var criteria = new AbsoluteConvergence(iterations: 10, tolerance: 0.1);

            int progress = 1;

            do
            {
                // Do some processing...


                // Update current iteration information:
                criteria.NewValue = 12345.6 / progress++;

            } while (!criteria.HasConverged);


            // The method will converge after reaching the 
            // maximum of 10 iterations with a final value
            // of 1371.73:

            int iterations = criteria.CurrentIteration; // 10
            double value = criteria.OldValue; // 1371.7333333


            Assert.AreEqual(10, criteria.CurrentIteration);
            Assert.AreEqual(1371.7333333333333, criteria.OldValue);
        }

        [TestMethod]
        public void AbsoluteConvergenceConstructorTest2()
        {
            var criteria = new AbsoluteConvergence(iterations: 10, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(4, criteria.CurrentIteration);
            Assert.AreEqual(-5, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-6, Math.Log10(criteria.NewValue));
        }

        [TestMethod]
        public void AbsoluteConvergenceConstructorTest3()
        {
            var criteria = new AbsoluteConvergence(iterations: 1, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(1, criteria.CurrentIteration);
            Assert.AreEqual(-2, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-3, Math.Log10(criteria.NewValue));
        }

        [TestMethod]
        public void AbsoluteConvergenceConstructorTest4()
        {
            var criteria = new AbsoluteConvergence(iterations: 0, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(4, criteria.CurrentIteration);
            Assert.AreEqual(-5, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-6, Math.Log10(criteria.NewValue));
        }

        [TestMethod]
        public void AbsoluteConvergenceConstructorTest5()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new AbsoluteConvergence(iterations: -10, tolerance: 1e-10, startValue: 1));
        }
    }
}
