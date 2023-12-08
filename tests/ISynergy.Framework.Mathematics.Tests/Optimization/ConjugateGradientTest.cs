﻿namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class ConjugateGradientTest
{

    [TestMethod]
    public void MinimizeTest()
    {
        Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
        Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

        Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


        int n = 2; // number of variables
        double[] initial = { -1.2, 1 };

        ConjugateGradient cg = new(n, f, g);
        cg.Method = ConjugateGradientMethod.FletcherReeves;

        Assert.IsTrue(cg.Minimize(initial));
        double actual = cg.Value;
        double expected = 0;
        Assert.AreEqual(expected, actual, 1e-6);

        double[] result = cg.Solution;

        Assert.AreEqual(127, cg.Evaluations);
        Assert.AreEqual(34, cg.Iterations);
        Assert.AreEqual(1.0, result[0], 1e-3);
        Assert.AreEqual(1.0, result[1], 1e-3);
        Assert.IsFalse(double.IsNaN(result[0]));
        Assert.IsFalse(double.IsNaN(result[1]));

        double y = f(result);
        double[] d = g(result);

        Assert.AreEqual(0.0, y, 1e-6);
        Assert.AreEqual(0.0, d[0], 1e-3);
        Assert.AreEqual(0.0, d[1], 1e-3);

        Assert.IsFalse(double.IsNaN(y));
        Assert.IsFalse(double.IsNaN(d[0]));
        Assert.IsFalse(double.IsNaN(d[1]));
    }

    [TestMethod]
    public void MinimizeTest2()
    {
        Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
        Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

        Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


        int n = 2; // number of variables
        double[] initial = { -1.2, 1 };

        ConjugateGradient cg = new(n, f, g);
        cg.Method = ConjugateGradientMethod.PolakRibiere;

        Assert.IsTrue(cg.Minimize(initial));
        double actual = cg.Value;
        double expected = 0;
        Assert.AreEqual(expected, actual, 1e-6);

        double[] result = cg.Solution;

        Assert.AreEqual(125, cg.Evaluations);
        Assert.AreEqual(32, cg.Iterations);
        Assert.AreEqual(1.0, result[0], 1e-3);
        Assert.AreEqual(1.0, result[1], 1e-3);
        Assert.IsFalse(double.IsNaN(result[0]));
        Assert.IsFalse(double.IsNaN(result[1]));

        double y = f(result);
        double[] d = g(result);

        Assert.AreEqual(0.0, y, 1e-6);
        Assert.AreEqual(0.0, d[0], 1e-3);
        Assert.AreEqual(0.0, d[1], 1e-3);

        Assert.IsFalse(double.IsNaN(y));
        Assert.IsFalse(double.IsNaN(d[0]));
        Assert.IsFalse(double.IsNaN(d[1]));
    }

    [TestMethod]
    public void MinimizeTest3()
    {
        Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
        Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

        Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


        int n = 2; // number of variables
        double[] initial = { -1.2, 1 };

        ConjugateGradient cg = new(n, f, g);
        cg.Method = ConjugateGradientMethod.PositivePolakRibiere;

        Assert.IsTrue(cg.Minimize(initial));
        double actual = cg.Value;
        double expected = 0;
        Assert.AreEqual(expected, actual, 1e-6);

        double[] result = cg.Solution;

        Assert.AreEqual(143, cg.Evaluations);
        Assert.AreEqual(28, cg.Iterations);
        Assert.AreEqual(1.0, result[0], 1e-3);
        Assert.AreEqual(1.0, result[1], 1e-3);
        Assert.IsFalse(double.IsNaN(result[0]));
        Assert.IsFalse(double.IsNaN(result[1]));

        double y = f(result);
        double[] d = g(result);

        Assert.AreEqual(0.0, y, 1e-6);
        Assert.AreEqual(0.0, d[0], 1e-3);
        Assert.AreEqual(0.0, d[1], 1e-3);

        Assert.IsFalse(double.IsNaN(y));
        Assert.IsFalse(double.IsNaN(d[0]));
        Assert.IsFalse(double.IsNaN(d[1]));
    }
}
