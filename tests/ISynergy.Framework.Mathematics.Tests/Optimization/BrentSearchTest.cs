using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Optimization;
[TestClass]
public class BrentSearchTest
{

    [TestMethod]
    public void ConstructorTest()
    {
        #region doc_example
        // Suppose we were given the function x³ + 2x² - 10x + 1 and 
        // we have to find its root, maximum and minimum inside 
        // the interval [-4, 2]. First, we express this function
        // as a lambda expression:
        Func<double, double> function = x => x * x * x + 2 * x * x - 10 * x + 1;

        // And now we can create the search algorithm:
        BrentSearch search = new(function, -4, 2);

        // Finally, we can query the information we need
        bool success1 = search.Maximize();  // should be true
        double max = search.Solution;       // occurs at -2.61

        bool success2 = search.Minimize();   // should be true  
        double min = search.Solution;       // occurs at  1.28

        bool success3 = search.FindRoot();  // should be true 
        double root = search.Solution;      // occurs at  0.10
        double value = search.Value;        // should be zero
        #endregion

        Assert.IsTrue(success1);
        Assert.IsTrue(success2);
        Assert.IsTrue(success3);
        Assert.AreEqual(-2.6103173073566239, max);
        Assert.AreEqual(1.2769839857480398, min);
        Assert.AreEqual(0.10219566016872624, root);
        Assert.AreEqual(0, value, 1e-5);
    }


    [TestMethod]
    public void CreateSearchWithParametersSetPropertiesSetsCorrectly()
    {
        Func<double, double> f = x => 2 * x + 4;

        double lowerBound = -3;
        double upperBound = +5;
        double tolerance = 1e-9;
        int maxIterations = 20;

        BrentSearch sut = new(f, lowerBound, upperBound, tolerance, maxIterations);

        Assert.AreSame(f, sut.Function);
        Assert.AreEqual(lowerBound, sut.LowerBound);
        Assert.AreEqual(upperBound, sut.UpperBound);
        Assert.AreEqual(tolerance, sut.Tolerance);
        Assert.AreEqual(maxIterations, sut.MaxIterations);
    }


    [TestMethod]
    public void FindRootTest()
    {
        //  Example from http://en.wikipedia.org/wiki/Brent%27s_method

        Func<double, double> f = x => (x + 3) * Math.Pow(x - 1, 2);
        double a = -4;
        double b = 4 / 3.0;

        double expected = -3;
        double actual = BrentSearch.FindRoot(f, a, b);

        Assert.AreEqual(expected, actual, 1e-6);
        Assert.IsFalse(double.IsNaN(actual));

        BrentSearch search = new(f, a, b);
        bool isSuccess = search.FindRoot();

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(BrentSearchStatus.Success, search.Status);
        Assert.AreEqual(expected, search.Solution, 1e-6);
        Assert.IsTrue(Math.Abs(search.Value) < 1e-5);
    }


    [TestMethod]
    public void FindTest()
    {
        //  Example from http://en.wikipedia.org/wiki/Brent%27s_method
        double value = 10;

        Func<double, double> f = x => (x + 3) * Math.Pow(x - 1, 2) + value;
        double a = -4;
        double b = 4 / 3.0;

        double expected = -3;
        double actual = BrentSearch.Find(f, value, a, b);

        Assert.AreEqual(expected, actual, 1e-6);
        Assert.AreEqual(value, f(actual), 1e-5);

        BrentSearch search = new(f, a, b);
        bool isSuccess = search.Find(value);

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(BrentSearchStatus.Success, search.Status);
        Assert.AreEqual(expected, search.Solution, 1e-6);
        Assert.AreEqual(value, search.Value, 1e-5);
    }


    [TestMethod]
    public void MaximizeTest()
    {
        Func<double, double> f = x => -2 * x * x - 3 * x + 5;

        double expected = -3 / 4d;
        double actual = BrentSearch.Maximize(f, -200, +200);

        Assert.AreEqual(expected, actual, 1e-10);


        BrentSearch search = new(f, -200, 200);
        bool isSuccess = search.Maximize();

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(BrentSearchStatus.Success, search.Status);
        Assert.AreEqual(expected, search.Solution, 1e-10);
        Assert.AreEqual(f(expected), search.Value, double.Epsilon);
    }


    [TestMethod]
    public void MinimizeTest()
    {
        Func<double, double> f = x => 2 * x * x - 3 * x + 5;

        double expected = 3 / 4d;
        double actual = BrentSearch.Minimize(f, -200, +200);

        Assert.AreEqual(expected, actual, 1e-10);


        BrentSearch search = new(f, -200, 200);
        bool isSuccess = search.Minimize();

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(BrentSearchStatus.Success, search.Status);
        Assert.AreEqual(expected, search.Solution, 1e-10);
        Assert.AreEqual(f(expected), search.Value, double.Epsilon);
    }


    [TestMethod]
    public void FindRootNotBoundedThrowsConvergenceException()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -1;
        double b = +1;

        Assert.Throws<ConvergenceException>(() =>
        {
            double actual = BrentSearch.FindRoot(f, a, b);
        });
    }


    [TestMethod]
    public void FindRootNotBoundedFailsToSolve()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -1;
        double b = +1;

        BrentSearch search = new(f, a, b);
        bool isSuccess = search.FindRoot();

        Assert.AreEqual(false, isSuccess);
        Assert.AreEqual(BrentSearchStatus.RootNotBracketed, search.Status);
    }


    [TestMethod]
    public void FindRootBoundedTwiceThrowsConvergenceException()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -3;
        double b = +3;

        Assert.Throws<ConvergenceException>(() =>
        {
            double actual = BrentSearch.FindRoot(f, a, b);
        });
    }


    [TestMethod]
    public void FindRootBoundedTwiceFailsToSolve()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -3;
        double b = +3;

        BrentSearch search = new(f, a, b);
        bool isSuccess = search.FindRoot();

        Assert.AreEqual(false, isSuccess);
        Assert.AreEqual(BrentSearchStatus.RootNotBracketed, search.Status);
    }


    [TestMethod]
    public void FindRootWithLowMaxIterationsThrowsConvergenceException()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -1;
        double b = +3000;

        Assert.Throws<ConvergenceException>(() =>
        {
            double actual = BrentSearch.FindRoot(f, a, b, maxIterations: 5);
        });
    }


    [TestMethod]
    public void FindRootWithLowMaxIterationsFailsToSolveButGivesLastKnownSolution()
    {
        Func<double, double> f = x => (x + 2) * (x - 2);
        double a = -1;
        double b = +3000;

        BrentSearch search = new(f, a, b, maxIterations: 5);
        bool isSuccess = search.FindRoot();

        Assert.AreEqual(false, isSuccess);
        Assert.AreEqual(BrentSearchStatus.MaxIterationsReached, search.Status);

        Assert.IsTrue(search.Solution > a && search.Solution < b);
    }


    [TestMethod]
    public void MaximizeWithLowMaxIterationsThrowsConvergenceException()
    {
        Func<double, double> f = x => -2 * x * x - 3 * x + 5;
        double a = -200;
        double b = +200;

        Assert.Throws<ConvergenceException>(() =>
        {
            double actual = BrentSearch.Maximize(f, a, b, maxIterations: 10);
        });
    }


    [TestMethod]
    public void MaximizeWithLowMaxIterationsFailsToSolveButGivesLastKnownSolution()
    {
        Func<double, double> f = x => -2 * x * x - 3 * x + 5;
        double a = -200;
        double b = +200;

        BrentSearch search = new(f, a, b, maxIterations: 10);
        bool isSuccess = search.Maximize();

        Assert.AreEqual(false, isSuccess);
        Assert.AreEqual(BrentSearchStatus.MaxIterationsReached, search.Status);

        Assert.IsTrue(search.Solution > a && search.Solution < b);
    }
}
