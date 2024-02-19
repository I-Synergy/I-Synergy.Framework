using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Optimization;
using ISynergy.Framework.Mathematics.Optimization.Base;
using ISynergy.Framework.Mathematics.Optimization.Constrained;
using ISynergy.Framework.Mathematics.Optimization.Constrained.Constraints;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using ISynergy.Framework.Mathematics.Random;
using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Optimization;
[TestClass]
public class AugmentedLagrangianTest
{

    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest1()
    {
        Generator.Seed = 0;

        // min 100(y-x*x)²+(1-x)²
        //
        // s.t.  x <= 0
        //       y <= 0
        //

        NonlinearObjectiveFunction f = new(2,

            function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

            gradient: (x) =>
            [
                2.0 * (200.0 * x[0]*x[0]*x[0] - 200.0 * x[0] * x[1] + x[0] - 1), // df/dx
                200 * (x[1] - x[0]*x[0])                                         // df/dy
            ]

        );


        List<NonlinearConstraint> constraints =
        [
            new NonlinearConstraint(f,

                function: (x) => x[0],
                gradient: (x) => [1.0, 0.0],

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 0
            ),
            new NonlinearConstraint(f,

                function: (x) => x[1],
                gradient: (x) => [0.0, 1.0],

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 0
            ),
        ];

        AugmentedLagrangian solver = new(f, constraints);

        Assert.IsTrue(solver.Minimize());
        double minValue = solver.Value;

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.AreEqual(1, minValue, 1e-5);
        Assert.AreEqual(0, solver.Solution[0], 1e-5);
        Assert.AreEqual(0, solver.Solution[1], 1e-5);
    }


    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest2()
    {
        // min 100(y-x*x)²+(1-x)²
        //
        // s.t.  x >= 0
        //       y >= 0
        //

        NonlinearObjectiveFunction f = new(2,

            function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

            gradient: (x) =>
            [
                2.0 * (200.0 * Math.Pow(x[0], 3) - 200.0 * x[0] * x[1] + x[0] - 1), // df/dx
                200 * (x[1] - x[0]*x[0])                                            // df/dy
            ]

        );


        List<NonlinearConstraint> constraints =
        [
            new NonlinearConstraint(f,

                function: (x) => x[0],
                gradient: (x) => [1.0, 0.0],

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0
            ),
            new NonlinearConstraint(f,

                function: (x) => x[1],
                gradient: (x) => [0.0, 1.0],

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0
            ),
        ];

        AugmentedLagrangian solver = new(f, constraints);

        Assert.IsTrue(solver.Minimize());
        double minValue = solver.Value;

        Assert.AreEqual(0, minValue, 1e-10);
        Assert.AreEqual(1, solver.Solution[0], 1e-5);
        Assert.AreEqual(1, solver.Solution[1], 1e-5);

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.IsFalse(double.IsNaN(solver.Solution[0]));
        Assert.IsFalse(double.IsNaN(solver.Solution[1]));
    }

#if !NET35
    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest3()
    {
        // min x*y+ y*z
        //
        // s.t.  x^2 - y^2 + z^2 - 2  >= 0
        //       x^2 + y^2 + z^2 - 10 <= 0
        //

        double x = 0, y = 0, z = 0;

        NonlinearObjectiveFunction f = new(

            function: () => x * y + y * z,

            gradient: () => new[]
            {
                y,     // df/dx
                x + z, // df/dy
                y,     // df/dz
            }

        );


        List<NonlinearConstraint> constraints =
        [
            new NonlinearConstraint(f,

                function: () => x * x - y * y + z * z,
                gradient: () => new[] { 2 * x, -2 * y, 2 * z },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 2
            ),
            new NonlinearConstraint(f,

                function: () => x * x + y * y + z * z,
                gradient: () => new[] { 2 * x, 2 * y, 2 * z },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 10
            ),
        ];

        AugmentedLagrangian solver = new(f, constraints);

        solver.Solution[0] = 1;
        solver.Solution[1] = 1;
        solver.Solution[2] = 1;

        bool success = solver.Minimize();
        double minValue = solver.Value;
        Assert.IsTrue(success);

        Assert.AreEqual(-6.9, minValue, 1e-1);
        Assert.AreEqual(+1.73, solver.Solution[0], 1e-2);
        Assert.AreEqual(-2.00, solver.Solution[1], 1e-2);
        Assert.AreEqual(+1.73, solver.Solution[2], 1e-2);

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.IsFalse(double.IsNaN(solver.Solution[0]));
        Assert.IsFalse(double.IsNaN(solver.Solution[1]));
        Assert.IsFalse(double.IsNaN(solver.Solution[2]));

    }

    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest4()
    {
        // min x*y+ y*z
        //
        // s.t.  x^2 - y^2 + z^2 - 2  >= 0
        //       x^2 + y^2 + z^2 - 10 <= 0
        //       x   + y               = 1
        //

        double x = 0, y = 0, z = 0;

        NonlinearObjectiveFunction f = new(

            function: () => x * y + y * z,

            gradient: () => new[]
            {
                y,     // df/dx
                x + z, // df/dy
                y,     // df/dz
            }

        );


        List<NonlinearConstraint> constraints =
        [
            new NonlinearConstraint(f,

                function: () => x * x - y * y + z * z,
                gradient: () => new[] { 2 * x, -2 * y, 2 * z },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 2
            ),
            new NonlinearConstraint(f,

                function: () => x * x + y * y + z * z,
                gradient: () => new[] { 2 * x, 2 * y, 2 * z },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 10
            ),
            new NonlinearConstraint(f,

                function: () => x + y,
                gradient: () => new[] { 1.0, 1.0, 0.0 },

                shouldBe: ConstraintType.EqualTo, value: 1
            ),
        ];

        foreach (NonlinearConstraint c in constraints)
            c.Tolerance = 1e-5;

        AugmentedLagrangian solver = new(f, constraints);

        solver.Solution[0] = 1;
        solver.Solution[1] = 1;
        solver.Solution[2] = 1;

        Assert.IsTrue(solver.Minimize());
        double minValue = solver.Value;

        Assert.AreEqual(1, solver.Solution[0] + solver.Solution[1], 1e-4);

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.IsFalse(double.IsNaN(solver.Solution[0]));
        Assert.IsFalse(double.IsNaN(solver.Solution[1]));
        Assert.IsFalse(double.IsNaN(solver.Solution[2]));
    }

    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest5()
    {
        #region doc_lambda
        // Suppose we would like to minimize the following function:
        //
        //    f(x,y) = min 100(y-x²)²+(1-x)²
        //
        // Subject to the constraints
        //
        //    x >= 0  (x must be positive)
        //    y >= 0  (y must be positive)
        //

        // First, let's declare some symbolic variables
        double x = 0, y = 0; // (values do not matter)

        // Now, we create an objective function
        NonlinearObjectiveFunction f = new(

            // This is the objective function:  f(x,y) = min 100(y-x²)²+(1-x)²
            function: () => 100 * Math.Pow(y - x * x, 2) + Math.Pow(1 - x, 2),

            // And this is the vector gradient for the same function:
            gradient: () => new[]
            {
                2 * (200 * Math.Pow(x, 3) - 200 * x * y + x - 1), // df/dx = 2(200x³-200xy+x-1)
                200 * (y - x*x)                                   // df/dy = 200(y-x²)
            }
        );

        // Now we can start stating the constraints
        List<NonlinearConstraint> constraints =
        [
            // Add the non-negativity constraint for x
            new NonlinearConstraint(f,
                // 1st constraint: x should be greater than or equal to 0
                function: () => x,
                shouldBe: ConstraintType.GreaterThanOrEqualTo,
                value: 0,
                gradient: () => new[] { 1.0, 0.0 }
            ),

            // Add the non-negativity constraint for y
            new NonlinearConstraint(f,
                // 2nd constraint: y should be greater than or equal to 0
                function: () => y,
                shouldBe: ConstraintType.GreaterThanOrEqualTo,
                value: 0,
                gradient: () => new[] { 0.0, 1.0 }
            )
        ];

        // Finally, we create the non-linear programming solver
        AugmentedLagrangian solver = new(f, constraints);

        // And attempt to find a minimum
        bool success = solver.Minimize();

        // The solution found was { 1, 1 }
        double[] solution = solver.Solution;

        // with the minimum value zero.
        double minValue = solver.Value;
        #endregion

        Assert.IsTrue(success);
        Assert.AreEqual(0, minValue, 1e-10);
        Assert.AreEqual(1, solver.Solution[0], 1e-6);
        Assert.AreEqual(1, solver.Solution[1], 1e-6);

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.IsFalse(double.IsNaN(solver.Solution[0]));
        Assert.IsFalse(double.IsNaN(solver.Solution[1]));
    }

    [TestMethod]
    public void solve_vectors()
    {
        #region doc_vector
        // Suppose we would like to minimize the following function:
        //
        //    f(x,y) = min 100(y-x²)²+(1-x)²
        //
        // Subject to the constraints
        //
        //    x >= 0  (x must be positive)
        //    y >= 0  (y must be positive)
        //

        // Now, we can create an objective function using vectors
        NonlinearObjectiveFunction f = new(numberOfVariables: 2,

            // This is the objective function:  f(x,y) = min 100(y-x²)²+(1-x)²
            function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

            // And this is the vector gradient for the same function:
            gradient: (x) =>
            [
                2 * (200 * Math.Pow(x[0], 3) - 200 * x[0] * x[1] + x[0] - 1), // df/dx = 2(200x³-200xy+x-1)
                200 * (x[1] - x[0]*x[0])                                      // df/dy = 200(y-x²)
            ]
        );

        // As before, we state the constraints. However, to illustrate the flexibility
        // of the AugmentedLagrangian, we shall use LinearConstraints to constrain the problem.
        double[,] a = Matrix.Identity(2); // Set up the constraint matrix...
        double[] b = Vector.Zeros(2);     // ...and the values they must be greater than
        int numberOfEqualities = 0;
        LinearConstraintCollection linearConstraints = LinearConstraintCollection.Create(a, b, numberOfEqualities);

        // Finally, we create the non-linear programming solver
        AugmentedLagrangian solver = new(f, linearConstraints);

        // And attempt to find a minimum
        bool success = solver.Minimize();

        // The solution found was { 1, 1 }
        double[] solution = solver.Solution;

        // with the minimum value zero.
        double minValue = solver.Value;
        #endregion

        Assert.IsTrue(success);
        Assert.AreEqual(0, minValue, 1e-10);
        Assert.AreEqual(1, solver.Solution[0], 1e-6);
        Assert.AreEqual(1, solver.Solution[1], 1e-6);

        Assert.IsFalse(double.IsNaN(minValue));
        Assert.IsFalse(double.IsNaN(solver.Solution[0]));
        Assert.IsFalse(double.IsNaN(solver.Solution[1]));
    }

    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest6()
    {
        test1(new ConjugateGradient(2), 5e-3);
        test1(new BoundedBroydenFletcherGoldfarbShanno(2), 1e-4);
        test1(new BroydenFletcherGoldfarbShanno(2), 1e-4);
        //test1(new ResilientBackpropagation(2), 1e-2);
    }

    [TestMethod]
    public void AugmentedLagrangianSolverConstructorTest7()
    {
        test2(new ConjugateGradient(2));
        test2(new BroydenFletcherGoldfarbShanno(2));
    }

    private static void test1(IGradientOptimizationMethod inner, double tol)
    {
        // maximize 2x + 3y, s.t. 2x² + 2y² <= 50 and x+y = 1

        // Max x' * c
        //  x

        // s.t. x' * A * x <= k
        //      x' * i     = 1
        // lower_bound < x < upper_bound

        double[] c = [2, 3];
        double[,] A = { { 2, 0 }, { 0, 2 } };
        double k = 50;

        // Create the objective function
        NonlinearObjectiveFunction objective = new(2,
            function: (x) => x.Dot(c),
            gradient: (x) => c
        );

        // Test objective
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                double expected = i * 2 + j * 3;
                double actual = objective.Function([i, j]);
                Assert.AreEqual(expected, actual);
            }
        }


        // Create the optimization constraints
        List<NonlinearConstraint> constraints =
        [
            new QuadraticConstraint(objective,
                quadraticTerms: A,
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: k
            ),
            new NonlinearConstraint(objective,
                function: (x) => x.Sum(),
                gradient: (x) => [1.0, 1.0],
                shouldBe: ConstraintType.EqualTo, value: 1
            ),
        ];


        // Test first constraint
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                double expected = i * (2 * i + 0 * j) + j * (0 * i + 2 * j);
                double actual = constraints[0].Function([i, j]);
                Assert.AreEqual(expected, actual);
            }
        }


        // Test second constraint
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                double expected = i + j;
                double actual = constraints[1].Function([i, j]);
                Assert.AreEqual(expected, actual);
            }
        }

        foreach (NonlinearConstraint constraint in constraints)
            constraint.Tolerance = 1e-7;


        AugmentedLagrangian solver =
            new(inner, objective, constraints);

        Assert.AreEqual(inner, solver.Optimizer);

        Assert.IsTrue(solver.Maximize());
        double maxValue = solver.Value;

        Assert.AreEqual(6, maxValue, tol);
        Assert.AreEqual(-3, solver.Solution[0], tol);
        Assert.AreEqual(4, solver.Solution[1], tol);
    }

    private static void test2(IGradientOptimizationMethod inner)
    {
        Generator.Seed = 0;

        // maximize 2x + 3y, s.t. 2x² + 2y² <= 50
        //
        // http://www.wolframalpha.com/input/?i=max+2x+%2B+3y%2C+s.t.+2x%C2%B2+%2B+2y%C2%B2+%3C%3D+50

        // Max x' * c
        //  x

        // s.t. x' * A * x <= k
        //      x' * i     = 1
        // lower_bound < x < upper_bound

        double[] c = [2, 3];
        double[,] A = { { 2, 0 }, { 0, 2 } };
        double k = 50;

        // Create the objective function
        NonlinearObjectiveFunction objective = new(2,
            function: (x) => x.Dot(c),
            gradient: (x) => c
        );

        // Test objective
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                double expected = i * 2 + j * 3;
                double actual = objective.Function([i, j]);
                Assert.AreEqual(expected, actual);
            }
        }


        // Create the optimization constraints
        List<NonlinearConstraint> constraints =
        [
            new QuadraticConstraint(objective,
                quadraticTerms: A,
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: k
            ),
        ];


        // Test first constraint
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                double[] input = [i, j];

                double expected = i * (2 * i + 0 * j) + j * (0 * i + 2 * j);
                double actual = constraints[0].Function(input);
                Assert.AreEqual(expected, actual);
            }
        }


        // Create the solver algorithm
        AugmentedLagrangian solver =
            new(inner, objective, constraints);

        Assert.AreEqual(inner, solver.Optimizer);

        Assert.IsTrue(solver.Maximize());
        double maxValue = solver.Value;

        Assert.AreEqual(18.02, maxValue, 1e-2);
        Assert.AreEqual(2.77, solver.Solution[0], 1e-2);
        Assert.AreEqual(4.16, solver.Solution[1], 1e-2);
    }

#endif


    [TestMethod]
    public void ConstructorTest2()
    {
        Generator.Seed = 0;

        NonlinearObjectiveFunction function = new(2,
            function: x => x[0] * x[1],
            gradient: x => [x[1], x[0]]);

        NonlinearConstraint[] constraints =
        [
            new NonlinearConstraint(function,
                function: x => 1.0 - x[0] * x[0] - x[1] * x[1],
                gradient: x => [-2 * x[0], -2 * x[1]]),
            new NonlinearConstraint(function,
                function: x => x[0],
                gradient: x => [1.0, 0.0])
        ];

        ConjugateGradient target = new(2);
        target.Tolerance = 0;
        AugmentedLagrangian solver = new(target, function, constraints);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;

        double[] solution = solver.Solution;

        double sqrthalf = Math.Sqrt(0.5);

        Assert.AreEqual(-0.5, minimum, 1e-5);
        Assert.AreEqual(sqrthalf, solution[0], 1e-5);
        Assert.AreEqual(-sqrthalf, solution[1], 1e-5);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void ConstructorTest3()
    {
        // minimize f(x) = x*y*z, 
        // s.t. 
        //   
        //    1 - x² - 2y² - 3z² > 0
        //    x > 0,
        //    y > 0
        //

        // Easy three dimensional minimization in ellipsoid.
        NonlinearObjectiveFunction function = new(3,
            function: x => x[0] * x[1] * x[2],
            gradient: x => [x[1] * x[2], x[0] * x[2], x[0] * x[1]]);

        NonlinearConstraint[] constraints =
        [
            new NonlinearConstraint(3,
                function: x =>  1.0 - x[0] * x[0] - 2.0 * x[1] * x[1] - 3.0 * x[2] * x[2],
                gradient: x => [-2.0 * x[0],  -4.0 * x[1], -6.0 * x[2]]),
            new NonlinearConstraint(3,
                function: x =>  x[0],
                gradient: x => [1.0, 0, 0]),
            new NonlinearConstraint(3,
                function: x =>  x[1],
                gradient: x => [0, 1.0, 0]),
            new NonlinearConstraint(3,
                function: x =>  -x[2],
                gradient: x => [0, 0, -1.0])
        ];

        for (int i = 0; i < constraints.Length; i++)
        {
            Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[i].ShouldBe);
            Assert.AreEqual(0, constraints[i].Value);
        }

        BroydenFletcherGoldfarbShanno inner = new(3);
        inner.LineSearch = LineSearch.BacktrackingArmijo;
        inner.Corrections = 10;

        AugmentedLagrangian solver = new(inner, function, constraints);

        Assert.AreEqual(inner, solver.Optimizer);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        double[] expected =
        [
            1.0 / Math.Sqrt(3.0), 1.0 / Math.Sqrt(6.0), -1.0 / 3.0
        ];


        for (int i = 0; i < expected.Length; i++)
            Assert.AreEqual(expected[i], solver.Solution[i], 1e-3);
        Assert.AreEqual(-0.078567420132031968, minimum, 1e-4);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void ConstructorTest4()
    {
        // Example code from 
        // https://groups.google.com/forum/#!topic/accord-net/an0sJGGrOuU

        int nVariablesTest = 4; // number of variables
        int nConstraintsTest = 2; // number of constraints
        double constraintsTolerance = 1e-100;
        double[,] ATest = new double[,] { { 1, 2, 3, 4 }, { 0, 4, 3, 1 } }; // arbitary A matrix.  A*X =  b
        double[,] bTest = new double[,] { { 0 }, { 2 } }; // arbitary A matrix.  A*X =  b

        double[,] XSolve = ATest.Solve(bTest);  // uses the pseudoinverse to minimise norm(X) subject to A*X =  b

        // recreate Solve function using AugmentedLagrangian
        NonlinearObjectiveFunction fTest = new(nVariablesTest, ds => ds.Dot(ds), ds => ds.Multiply(2.0)); // minimise norm(ds)

        List<NonlinearConstraint> nonlinearConstraintsTest = new(nConstraintsTest);  // linear constraints A*X = b
        for (int i = 0; i < nConstraintsTest; i++)
        {
            int j = i; // http://blogs.msdn.com/b/ericlippert/archive/2009/11/12/closing-over-the-loop-variable-considered-harmful.aspx
            nonlinearConstraintsTest.Add(new NonlinearConstraint(fTest, ds => ATest.GetRow(j).Dot(ds) - (double)bTest.GetValue(j, 0), ConstraintType.EqualTo, 0.0, ds => ATest.GetRow(j), constraintsTolerance));
        }

        ResilientBackpropagation innerSolverTest = new(nVariablesTest);
        innerSolverTest.Tolerance = constraintsTolerance;
        innerSolverTest.Iterations = 1000;
        AugmentedLagrangian solverTest = new(innerSolverTest, fTest, nonlinearConstraintsTest);
        solverTest.MaxEvaluations = 0;
        bool didMinimise = solverTest.Minimize();

        double[,] errorConstraintRelative = XSolve.Subtract(solverTest.Solution, 1).ElementwiseDivide(XSolve); // relative error between .Solve and .Minimize
        double[,] errorConstraintAbsolute = XSolve.Subtract(solverTest.Solution, 1); // absolute error between .Solve and .Minimize

        double[] errorConstraintsTest = new double[nConstraintsTest];
        for (int i = 0; i < nConstraintsTest; i++)
        {
            errorConstraintsTest[i] = nonlinearConstraintsTest[i].Function(solverTest.Solution);
        }
    }

    [TestMethod]
    public void constructorTest5()
    {
        // AugmentedLagrangian with NonlinearConstraints
        // have a Gradient NullReferenceException issue 
        // https://github.com/accord-net/framework/issues/177

        // Easy three dimensional minimization in ellipsoid.
        NonlinearObjectiveFunction function = new(3,
            function: x => x[0] * x[1] * x[2],
            gradient: x => [x[1] * x[2], x[0] * x[2], x[0] * x[1]]);

        NonlinearConstraint[] constraints =
        [
            new NonlinearConstraint(function,
                constraint: x =>  1.0 - x[0] * x[0] - 2.0 * x[1] * x[1] - 3.0 * x[2] * x[2] >= 0,
                gradient: x => [-2.0 * x[0],  -4.0 * x[1], -6.0 * x[2]]),
            new NonlinearConstraint(function,
                constraint: x =>  x[0] >= 0,
                gradient: x => [1.0, 0, 0]),
            new NonlinearConstraint(function,
                constraint: x =>  x[1] >= 0,
                gradient: x => [0, 1.0, 0]),
            new NonlinearConstraint(function,
                constraint: x =>  -x[2] >= 0,
                gradient: x => [0, 0, -1.0])
        ];

        for (int i = 0; i < constraints.Length; i++)
        {
            Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[i].ShouldBe);
            Assert.AreEqual(0, constraints[i].Value);
        }

        BroydenFletcherGoldfarbShanno inner = new(3);
        inner.LineSearch = LineSearch.BacktrackingArmijo;
        inner.Corrections = 10;

        AugmentedLagrangian solver = new(inner, function, constraints);

        Assert.AreEqual(inner, solver.Optimizer);

        Assert.IsTrue(solver.Minimize());
        double minimum = solver.Value;
        double[] solution = solver.Solution;

        double[] expected =
        [
            1.0 / Math.Sqrt(3.0), 1.0 / Math.Sqrt(6.0), -1.0 / 3.0
        ];


        for (int i = 0; i < expected.Length; i++)
            Assert.AreEqual(expected[i], solver.Solution[i], 1e-3);
        Assert.AreEqual(-0.078567420132031968, minimum, 1e-4);

        double expectedMinimum = function.Function(solver.Solution);
        Assert.AreEqual(expectedMinimum, minimum);
    }

    [TestMethod]
    public void AugmentedLagrangianSolverTest02()
    {
        // Ensure that the Accord.NET random generator starts from a particular fixed seed.
        Generator.Seed = 0;

        // The minimization problem is to minimize the function (x_0 - 1)^2 + (x_1 - 2.5)^2$ subject
        // to the five constraints $x_0 - 2x_1 +2 \ge 0$, $-x_0 - 2x_1 + 6 \ge0$, $-x_0 + 2x_1 + 2\ge0$,
        // $x_0\ge0$ and $x_1\ge0$.

        NonlinearObjectiveFunction f = new(2,
            function: (x) => (x[0] - 1.0) * (x[0] - 1.0) + (x[1] - 2.5) * (x[1] - 2.5),
            gradient: (x) => [2.0 * (x[0] - 1.0), 2.0 * (x[1] - 2.5)]);

        List<NonlinearConstraint> constraints =
        [
            // Add the constraint $x_1 - 2x_2 + 2 \ge0$.
            new NonlinearConstraint(f,
                function: (x) => x[0] - 2.0 * x[1] + 2.0,
                gradient: (x) => [1.0, -2.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $-x_0 - 2x_1 + 6 \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => -x[0] - 2.0 * x[1] + 6.0,
                gradient: (x) => [-1.0, -2.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $-x_0 + 2x_1 + 2 \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => -x[0] + 2.0 * x[1] + 2.0,
                gradient: (x) => [-1.0, 2.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $x_0  \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => x[0],
                gradient: (x) => [1.0, 0.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $x_1  \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => x[1],
                gradient: (x) => [0.0, 1.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
        ];

        AugmentedLagrangian solver = new(f, constraints);

        Assert.IsTrue(solver.Minimize());
        double minValue = solver.Value;

        Assert.IsFalse(double.IsNaN(minValue));

        // According to the example, the solution is $(1.4, 1.7)$.
        Assert.AreEqual(1.4, solver.Solution[0], 1e-5);
        Assert.AreEqual(1.7, solver.Solution[1], 1e-5);
    }


    [TestMethod]
    public void AugmentedLagrangianSolverTest03()
    {

        // Ensure that the Accord.NET random generator starts from a particular fixed seed.
        Generator.Seed = 0;

        // This problem is about minimizing $3x_0 - 4x_1$ over the elements of $[0, 1] \times[0, 1]$ summing to one.

        NonlinearObjectiveFunction f = new(2,
            function: (x) => 3.0 * x[0] - 4.0 * x[1],
            gradient: (x) => [3.0, -4.0]);

        List<NonlinearConstraint> constraints =
        [
            // Add the constraint $x_0 \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => x[0],
                gradient: (x) => [1.0, 0.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $x_0 \le 1$.
            new NonlinearConstraint(f,
                function: (x) => x[0],
                gradient: (x) => [1.0, 0.0],
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 1),
            // Add the constraint $x_1 \ge 0$.
            new NonlinearConstraint(f,
                function: (x) => x[1],
                gradient: (x) => [0.0, 1.0],
                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0),
            // Add the constraint $x_1 \le 1$.
            new NonlinearConstraint(f,
                function: (x) => x[1],
                gradient: (x) => [0.0, 1.0],
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 1),
            // Add the constraint $x_0 + x_1 = 1$.
            new NonlinearConstraint(f,
                function: (x) => x[0] + x[1],
                gradient: (x) => [1.0, 1.0],
                shouldBe: ConstraintType.EqualTo, value: 1),
        ];

        AugmentedLagrangian solver = new(f, constraints);

        Assert.IsTrue(solver.Minimize());
        double minValue = solver.Value;

        Assert.IsFalse(double.IsNaN(minValue));

        // The solution is $(0.0, 1.0)$.
        Assert.AreEqual(0.0, solver.Solution[0], 1e-5);
        Assert.AreEqual(1.0, solver.Solution[1], 1e-5);
    }
}
