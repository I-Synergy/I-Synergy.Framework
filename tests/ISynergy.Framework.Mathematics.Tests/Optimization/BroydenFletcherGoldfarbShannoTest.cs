namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
    using ISynergy.Framework.Mathematics.Random;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class BroydenFletcherGoldfarbShannoTest
    {


        [TestMethod]
        public void lbfgsTest()
        {
            Func<double[], double> f = rosenbrockFunction;
            Func<double[], double[]> g = rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            BroydenFletcherGoldfarbShanno lbfgs = new(n, f, g);

            double expected = 0;
            Assert.IsTrue(lbfgs.Minimize(initial));

            bool success = lbfgs.Minimize();
            double actual = lbfgs.Value;
            Assert.IsTrue(success);

            Assert.AreEqual(expected, actual, 1e-10);

            double[] result = lbfgs.Solution;

            Assert.AreEqual(1.0, result[0], 1e-5);
            Assert.AreEqual(1.0, result[1], 1e-5);

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0, y, 1e-10);
            Assert.AreEqual(0, d[0], 1e-6);
            Assert.AreEqual(0, d[1], 1e-6);
        }

        [TestMethod]
        public void lbfgsTest2()
        {
            #region doc_minimize
            // Ensure that results are reproducible
            ISynergy.Framework.Mathematics.Random.Generator.Seed = 0;

            // Suppose we would like to find the minimum of the function
            // 
            //   f(x,y)  =  -exp{-(x-1)²} - exp{-(y-2)²/2}
            //

            // First we need write down the function either as a named
            // method, an anonymous method or as a lambda function:

            Func<double[], double> f = (x) =>
                -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            // Now, we need to write its gradient, which is just the
            // vector of first partial derivatives del_f / del_x, as:
            //
            //   g(x,y)  =  { del f / del x, del f / del y }
            // 

            Func<double[], double[]> g = (x) => new double[]
            {
                // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };

            // Finally, we create a L-BFGS solver for the two variable problem:
            BroydenFletcherGoldfarbShanno lbfgs = new(numberOfVariables: 2)
            {
                Function = f,
                Gradient = g
            };

            // And then minimize the function:
            bool success = lbfgs.Minimize();     // should be true
            double minValue = lbfgs.Value;       // should be -2
            double[] solution = lbfgs.Solution;  // should be (1, 2)

            // The resultant minimum value should be -2, and the solution
            // vector should be { 1.0, 2.0 }. The answer can be checked on
            // Wolfram Alpha by clicking the following the link:

            // http://www.wolframalpha.com/input/?i=maximize+%28exp%28-%28x-1%29%C2%B2%29+%2B+exp%28-%28y-2%29%C2%B2%2F2%29%29
            #endregion

            Assert.IsTrue(success);
            double expected = -2;
            Assert.AreEqual(expected, minValue, 1e-10);

            Assert.AreEqual(1, solution[0], 1e-3);
            Assert.AreEqual(2, solution[1], 1e-3);
        }

        // The famous Rosenbrock Test function.
        public static double rosenbrockFunction(double[] x)
        {
            // f(x, y) = (1 - x)^2 + 100(y - x^2)^2
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];
            return b * b + 100 * a * a;
        }

        // Gradient of the Rosenbrock Test function.
        public static double[] rosenbrockGradient(double[] x)
        {
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];

            double f0 = -2 * b - 400 * x[0] * a;
            double f1 = 200 * a;

            return new[] { f0, f1 };
        }

        private static void createTestFunction(out Func<double[], double> f, out Func<double[], double[]> g)
        {
            // min f(x, y) = -exp(-(x-1)^2) - exp(-0.5*(y-2)^2)
            f = (x) => -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            g = (x) => new[]
            {
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };
        }

        [TestMethod]
        public void NoFunctionTest()
        {
            BroydenFletcherGoldfarbShanno target = new(2);

            Assert.ThrowsException<InvalidOperationException>(() => target.Minimize(), "");
        }

        [TestMethod]
        public void NoGradientTest()
        {
            BroydenFletcherGoldfarbShanno target = new(2)
            {
                Function = (x) => 0.0
            };

            Assert.IsTrue(target.Minimize());

            // The optimizer should use finite differences as the gradient
        }

        [TestMethod]
        public void WrongGradientSizeTest()
        {
            BroydenFletcherGoldfarbShanno target = new(2)
            {
                Function = (x) => 0.0,
                Gradient = (x) => new double[1]
            };

            Assert.ThrowsException<InvalidOperationException>(() => target.Minimize(), "");
        }

        [TestMethod]
        public void MutableGradientSizeTest()
        {
            BroydenFletcherGoldfarbShanno target = new(2)
            {
                Function = (x) => 0.0,
                Gradient = (x) => x
            };

            Assert.ThrowsException<InvalidOperationException>(() => target.Minimize(), "");
        }

        [TestMethod]
        public void ConstructorTest1()
        {
            Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Func<double[], double[]> gradient = x => new[] { 20 * (x[0] + 1), 2 * x[1] };

            BroydenFletcherGoldfarbShanno target = new(2)
            {
                Function = function,
                Gradient = gradient
            };

            Assert.IsTrue(target.Minimize());
            double minimum = target.Value;

            double[] solution = target.Solution;

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(target.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [TestMethod]
        public void lbfgsTest3()
        {
            Generator.Seed = 0;

            Func<double[], double> f;
            Func<double[], double[]> g;
            createExpDiff(out f, out g);

            int errors = 0;

            for (int i = 0; i < 10000; i++)
            {
                double[] start = Vector.Random(2, -1.0, 1.0);

                BroydenFletcherGoldfarbShanno lbfgs = new(numberOfVariables: 2, function: f, gradient: g);

                lbfgs.Minimize(start);
                double minValue = lbfgs.Value;
                double[] solution = lbfgs.Solution;

                double expected = -2;

                if (Math.Abs(expected - minValue) > 1e-3)
                    errors++;
            }

            Assert.IsTrue(errors < 800);
        }

        private static void createExpDiff(out Func<double[], double> f, out Func<double[], double[]> g)
        {
            f = (x) =>
                           -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            g = (x) => new double[]
            {
                // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };
        }



    }
}
