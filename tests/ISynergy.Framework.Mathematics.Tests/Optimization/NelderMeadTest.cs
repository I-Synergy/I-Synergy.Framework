namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics.Optimization;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

[TestClass]
public class NelderMeadTest
{

    [TestMethod]
    public void ConstructorTest1()
    {
        #region doc_min
        // Let's say we would like to find the minimum 
        // of the function "f(x) = 10 * (x+1)^2 + y^2".

        // In code, this means we would like to minimize:
        Func<double[], double> function = (double[] x) =>
            10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

        // We can do so using the NelderMead class:
        NelderMead solver = new(numberOfVariables: 2)
        {
            Function = function // f(x) = 10 * (x+1)^2 + y^2
        };

        // Now, we can minimize it with:
        bool success = solver.Minimize();

        // And get the solution vector using
        double[] solution = solver.Solution; // should be (-1, 1)

        // The minimum at this location would be:
        double minimum = solver.Value; // should be 0

        // Which can be double-checked against Wolfram Alpha if there is need:
        // https://www.wolframalpha.com/input/?i=min+10+*+(x%2B1)%5E2+%2B+y%5E2
        #endregion

        Assert.IsTrue(success);
        Assert.AreEqual(0, minimum, 1e-10);
        Assert.AreEqual(-1, solution[0], 1e-5);
        Assert.AreEqual(0, solution[1], 1e-5);

        double expectedMinimum = function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void ConstructorTest4()
    {
        NonlinearObjectiveFunction function = new(2, x =>
            Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

        NelderMead solver = new(function);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

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

        NelderMead solver = new(function);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        Assert.AreEqual(2, solution.Length);
        Assert.AreEqual(-0, minimum, 1e-6);
        Assert.AreEqual(-1, solution[0], 1e-3);
        Assert.AreEqual(+1, solution[1], 1e-3);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void SubspaceTest1()
    {
        NonlinearObjectiveFunction function = new(5, x =>
            10.0 * Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

        NelderMead solver = new(function);

        solver.NumberOfVariables = 2;

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        Assert.AreEqual(5, solution.Length);
        Assert.AreEqual(-0, minimum, 1e-6);
        Assert.AreEqual(-1, solution[0], 1e-3);
        Assert.AreEqual(+1, solution[1], 1e-3);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void gh335()
    {
        // https://github.com/accord-net/framework/issues/335
        Func<Double[], Double> eval = (val) =>
        {
            // WxMaxima command: plot3d(y^2+4*y+x^2-2*x,[x,-3,5], [y,-5,3],[grid,8,8]);
            Double x = val[0];
            Double y = val[1];
            Double ret = y * y + 4 * y + x * x - 2 * x;

            Debug.WriteLine("{2}; x={0}; y={1}", x, y, ret);

            return ret;
        };

        // This values are relevant for my RealWorld(TM) scenario
        Double[] init = new double[] { 0.5, 0 };

        NelderMead nm = new(2, eval);

        nm.Minimize(init);

        // Solution
        Assert.AreEqual(1, nm.Solution[0], 1e-7);
        Assert.AreEqual(-2, nm.Solution[1], 1e-6);
    }
}
