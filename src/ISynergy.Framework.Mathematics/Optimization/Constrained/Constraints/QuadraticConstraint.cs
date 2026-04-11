using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Matrices;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Optimization.Constrained.Constraints;

/// <summary>
///     Constraint with only quadratic terms.
/// </summary>
public class QuadraticConstraint : NonlinearConstraint, IConstraint
{
    /// <summary>
    ///     Constructs a new quadratic constraint in the form <c>x'Ax + x'b</c>.
    /// </summary>
    /// <param name="objective">The objective function to which this constraint refers.</param>
    /// <param name="quadraticTerms">The matrix of <c>A</c> quadratic terms.</param>
    /// <param name="linearTerms">The vector <c>b</c> of linear terms.</param>
    /// <param name="shouldBe">
    ///     How the left hand side of the constraint should be compared to
    ///     the given <paramref name="value" />.
    /// </param>
    /// <param name="value">The right hand side of the constraint equation.</param>
    /// <param name="withinTolerance">
    ///     The tolerance for violations of the constraint. Equality
    ///     constraints should set this to a small positive value. Default is 0.
    /// </param>
    public QuadraticConstraint(IObjectiveFunction objective,
        double[,] quadraticTerms, double[] linearTerms = null,
        ConstraintType shouldBe = ConstraintType.LesserThanOrEqualTo,
        double value = 0, double withinTolerance = 0.0)
    {
        var n = objective.NumberOfVariables;

        if (quadraticTerms is null)
            throw new ArgumentNullException("quadraticTerms");

        if (quadraticTerms.GetLength(0) != quadraticTerms.GetLength(1))
            throw new DimensionMismatchException("quadraticTerms", "Matrix must be square.");

        if (quadraticTerms.GetLength(0) != n)
            throw new DimensionMismatchException("quadraticTerms",
                "Matrix rows must match the number of variables in the objective function.");

        if (linearTerms is not null)
        {
            if (linearTerms.Length != n)
                throw new DimensionMismatchException("linearTerms",
                    "The length of the linear terms vector must match the " +
                    "number of variables in the objective function.");
        }
        else
        {
            linearTerms = new double[n];
        }

        QuadraticTerms = quadraticTerms;
        LinearTerms = linearTerms;

        Create(n, function, shouldBe, value, gradient, withinTolerance);
    }

    /// <summary>
    ///     Gets the matrix of <c>A</c> quadratic terms
    ///     for the constraint <c>x'Ax + x'b</c>.
    /// </summary>
    public double[,] QuadraticTerms { get; }

    /// <summary>
    ///     Gets the vector <c>b</c> of linear terms
    ///     for the constraint <c>x'Ax + x'b</c>.
    /// </summary>
    public double[] LinearTerms { get; }
    private double function(double[] x)
    {
        return x.DotAndDot(QuadraticTerms, x) + LinearTerms.Dot(x);
    }

    private double[] gradient(double[] x)
    {
        var g = new double[x.Length];

        for (var i = 0; i < x.Length; i++)
        {
            // Calculate quadratic terms
            g[i] = 2.0 * x[i] * QuadraticTerms[i, i];
            for (var j = 0; j < x.Length; j++)
                if (i != j)
                    g[i] += x[j] * (QuadraticTerms[i, j] + QuadraticTerms[j, i]);

            // Calculate for linear terms
            g[i] += LinearTerms[i];
        }

        return g;
    }
}