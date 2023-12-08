namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics.Convergence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class RelativeConvergenceTest
{
    [TestMethod]
    public void RelativeConvergenceConstructorTest()
    {
        RelativeConvergence criteria = new(iterations: 0, tolerance: 0.1);

        int progress = 1;

        do
        {
            // Do some processing...


            // Update current iteration information:
            criteria.NewValue = 12345.6 / progress++;

        } while (!criteria.HasConverged);


        // The method will converge after reaching the 
        // maximum of 11 iterations with a final value
        // of 1234.56:

        int iterations = criteria.CurrentIteration; // 11
        double value = criteria.OldValue; // 1234.56


        Assert.AreEqual(11, criteria.CurrentIteration);
        Assert.AreEqual(1234.56, criteria.OldValue);
    }

    [TestMethod]
    public void RelativeConvergenceConstructorTest2()
    {
        RelativeConvergence criteria = new(iterations: 10, tolerance: 1e-5, startValue: 1);
        criteria.CurrentIteration = -2;
        do
        {
            criteria.NewValue /= 10.0;
        } while (!criteria.HasConverged);

        Assert.AreEqual(10, criteria.CurrentIteration);
        Assert.AreEqual(-11, Math.Log10(criteria.OldValue));
        Assert.AreEqual(-12, Math.Log10(criteria.NewValue));
    }

    [TestMethod]
    public void RelativeConvergenceConstructorTest3()
    {
        RelativeConvergence criteria = new(iterations: 1, tolerance: 1e-5, startValue: 1);
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
    public void RelativeConvergenceConstructorTest4()
    {
        RelativeConvergence criteria = new(iterations: 0, tolerance: 1e-5, startValue: 1);
        criteria.CurrentIteration = -2;

        criteria.NewValue /= 10.0;
        Assert.AreEqual(0.9, criteria.Delta, 1e-10);
        Assert.AreEqual(0.9, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(0.1, criteria.NewValue, 1e-10);
        Assert.AreEqual(1, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue /= 10.0;
        Assert.AreEqual(0.09, criteria.Delta, 1e-10);
        Assert.AreEqual(0.9, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(0.01, criteria.NewValue, 1e-10);
        Assert.AreEqual(0.1, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue /= 10.0;
        Assert.AreEqual(0.009, criteria.Delta, 1e-10);
        Assert.AreEqual(0.9, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(0.001, criteria.NewValue, 1e-10);
        Assert.AreEqual(0.01, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue = criteria.NewValue * 1e-3;
        Assert.AreEqual(0.000999, criteria.Delta, 1e-10);
        Assert.AreEqual(0.999, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(1E-06, criteria.NewValue, 1e-10);
        Assert.AreEqual(0.001, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue = criteria.NewValue - (criteria.NewValue * 1e-3);
        Assert.AreEqual(1e-9, criteria.Delta, 1e-10);
        Assert.AreEqual(1e-3, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(9.9899999999999988E-07, criteria.NewValue, 1e-10);
        Assert.AreEqual(1E-06, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue = criteria.NewValue - (criteria.NewValue * 1e-5);
        Assert.AreEqual(9.999999999E-11, criteria.Delta, 1e-10);
        Assert.AreEqual(1e-5, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(9.9899000999999985E-07, criteria.NewValue, 1e-10);
        Assert.AreEqual(9.9899999999999988E-07, criteria.OldValue, 1e-10);
        Assert.IsFalse(criteria.HasConverged);

        criteria.NewValue = criteria.NewValue - (criteria.NewValue * 1e-6);
        Assert.AreEqual(9.999999999E-11, criteria.Delta, 1e-10);
        Assert.AreEqual(1.0000000000115289E-06, criteria.RelativeDelta, 1e-10);
        Assert.AreEqual(9.9898901100998984E-07, criteria.NewValue, 1e-10);
        Assert.AreEqual(9.9899000999999985E-07, criteria.OldValue, 1e-10);
        Assert.IsTrue(criteria.HasConverged);
    }

    [TestMethod]
    public void RelativeConvergenceConstructorTest5()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new RelativeConvergence(iterations: -10, tolerance: 1e-10, startValue: 1));
    }
}
