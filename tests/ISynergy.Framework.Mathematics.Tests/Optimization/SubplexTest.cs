namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics.Optimization;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class SubplexTest
{

    [TestMethod]
    public void ConstructorTest1()
    {
        Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
          x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

        Subplex solver = new(2, function);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;

        double[] solution = solver.Solution;

        Assert.AreEqual(0, minimum, 1e-10);
        Assert.AreEqual(-1, solution[0], 1e-5);
        Assert.AreEqual(0, solution[1], 1e-5);

        double expectedMinimum = function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void ConstructorTest4()
    {
        // Weak version of Rosenbrock's problem.
        NonlinearObjectiveFunction function = new(2, x =>
            Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

        Subplex solver = new(function);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        Assert.AreEqual(2, solution.Length);
        Assert.AreEqual(0, minimum, 1e-10);
        Assert.AreEqual(-1, solution[0], 1e-5);
        Assert.AreEqual(1, solution[1], 1e-4);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void ConstructorTest5()
    {
        NonlinearObjectiveFunction function = new(2, x =>
            10.0 * Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

        Subplex solver = new(function);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        Assert.AreEqual(-0, minimum, 1e-6);
        Assert.AreEqual(-1, solution[0], 1e-3);
        Assert.AreEqual(+1, solution[1], 1e-3);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

}
