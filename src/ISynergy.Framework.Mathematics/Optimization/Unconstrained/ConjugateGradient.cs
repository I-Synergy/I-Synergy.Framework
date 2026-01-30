using ISynergy.Framework.Core.Abstractions.Async;
using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Optimization.Base;

namespace ISynergy.Framework.Mathematics.Optimization.Unconstrained;

/// <summary>
///     Conjugate gradient direction update formula.
/// </summary>
public enum ConjugateGradientMethod
{
    /// <summary>
    ///     Fletcher-Reeves formula.
    /// </summary>
    FletcherReeves = 1,

    /// <summary>
    ///     Polak-Ribière formula.
    /// </summary>
    /// <remarks>
    ///     The Polak-Ribière is known to perform better for non-quadratic functions.
    /// </remarks>
    PolakRibiere = 2,

    /// <summary>
    ///     Polak-Ribière formula.
    /// </summary>
    /// <remarks>
    ///     The Polak-Ribière is known to perform better for non-quadratic functions.
    ///     The positive version B=max(0,Bpr) provides a direction reset automatically.
    /// </remarks>
    PositivePolakRibiere = 3
}

/// <summary>
///     Conjugate Gradient exit codes.
/// </summary>
public enum ConjugateGradientCode
{
    /// <summary>
    ///     Success.
    /// </summary>
    Success,

    /// <summary>
    ///     Invalid step size.
    /// </summary>
    StepSize = 1,

    /// <summary>
    ///     Descent direction was not obtained.
    /// </summary>
    DescentNotObtained = -2,

    /// <summary>
    ///     Rounding errors prevent further progress. There may not be a step
    ///     which satisfies the sufficient decrease and curvature conditions.
    ///     Tolerances may be too small.
    /// </summary>
    RoundingErrors = 6,

    /// <summary>
    ///     The step size has reached the upper bound.
    /// </summary>
    StepHigh = 5,

    /// <summary>
    ///     The step size has reached the lower bound.
    /// </summary>
    StepLow = 4,

    /// <summary>
    ///     Maximum number of function evaluations has been reached.
    /// </summary>
    MaximumEvaluations = 3,

    /// <summary>
    ///     Relative width of the interval of uncertainty is at machine precision.
    /// </summary>
    Precision = 2
}

/// <summary>
///     Conjugate Gradient (CG) optimization method.
/// </summary>
/// <remarks>
///     <para>
///         In mathematics, the conjugate gradient method is an algorithm for the numerical solution of
///         particular systems of linear equations, namely those whose matrix is symmetric and positive-
///         definite. The conjugate gradient method is an iterative method, so it can be applied to sparse
///         systems that are too large to be handled by direct methods. Such systems often arise when
///         numerically solving partial differential equations. The nonlinear conjugate gradient method
///         generalizes the conjugate gradient method to nonlinear optimization (Wikipedia, 2011).
///     </para>
///     <para>
///         T
///     </para>
///     <para>
///         The framework implementation of this method is based on the original FORTRAN source code
///         by Jorge Nocedal (see references below). The original FORTRAN source code of CG+ (for large
///         scale unconstrained problems) is available at http://users.eecs.northwestern.edu/~nocedal/CG+.html
///         and had been made freely available for educational or commercial use. The original authors
///         expect that all publications describing work using this software quote the (Gilbert and Nocedal, 1992)
///         reference given below.
///     </para>
///     <para>
///         References:
///         <list type="bullet">
///             <item>
///                 <description>
///                     <a href="http://users.eecs.northwestern.edu/~nocedal/CG+.html">
///                         J. C. Gilbert and J. Nocedal. Global Convergence Properties of Conjugate Gradient
///                         Methods for Optimization, (1992) SIAM J. on Optimization, 2, 1.
///                     </a>
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     Wikipedia contributors, "Nonlinear conjugate gradient method," Wikipedia, The Free
///                     Encyclopedia, http://en.wikipedia.org/w/index.php?title=Nonlinear_conjugate_gradient_method
///                     (accessed December 22, 2011).
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     Wikipedia contributors, "Conjugate gradient method," Wikipedia, The Free Encyclopedia,
///                     http://en.wikipedia.org/w/index.php?title=Conjugate_gradient_method
///                     (accessed December 22, 2011).
///                 </description>
///             </item>
///         </list>
///     </para>
/// </remarks>
/// <seealso cref="BroydenFletcherGoldfarbShanno" />
/// <seealso cref="ResilientBackpropagation" />
/// <seealso cref="BoundedBroydenFletcherGoldfarbShanno" />
/// <seealso cref="TrustRegionNewtonMethod" />
public class ConjugateGradient : BaseGradientOptimizationMethod,
    IGradientOptimizationMethod, IOptimizationMethod<ConjugateGradientCode>,
    ISupportsCancellation
{
    // TODO: Move to separate classes

    private bool brackt;

    private double[] d;
    private double dg2;
    private double dgtest;
    private double dgx;
    private double dgy;
    private readonly double epsilon = 1e-5;
    private double finit;
    private double ftest1;

    private readonly double ftol = 1e-4;
    private double fx;
    private double fy;

    private double[] g; // gradient at current solution
    private double[] gold;
    private readonly double gtol = 0.1;
    private int infoc;
    private readonly int maxfev = 40;
    private bool stage1;
    private double stmax;

    private double stmin;
    private readonly double stpmax = 1e20;
    private readonly double stpmin = 1e-20;
    private double stx;
    private double sty;
    private double[] w;
    private double width;
    private double width1;
    private readonly double xtol = 1e-17;

    /// <summary>
    ///     Creates a new instance of the CG optimization algorithm.
    /// </summary>
    public ConjugateGradient()
    {
    }

    /// <summary>
    ///     Creates a new instance of the CG optimization algorithm.
    /// </summary>
    /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
    public ConjugateGradient(int numberOfVariables)
        : base(numberOfVariables)
    {
    }

    /// <summary>
    ///     Creates a new instance of the CG optimization algorithm.
    /// </summary>
    /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
    /// <param name="function">The function to be optimized.</param>
    /// <param name="gradient">The gradient of the function.</param>
    public ConjugateGradient(int numberOfVariables,
        Func<double[], double> function, Func<double[], double[]> gradient)
        : base(numberOfVariables, function, gradient)
    {
    }
    /// <summary>
    ///     Gets or sets the relative difference threshold
    ///     to be used as stopping criteria between two
    ///     iterations. Default is 0 (iterate until convergence).
    /// </summary>
    public double Tolerance { get; set; } = 1e-5;

    /// <summary>
    ///     Gets or sets the maximum number of iterations
    ///     to be performed during optimization. Default
    ///     is 0 (iterate until convergence).
    /// </summary>
    public int MaxIterations { get; set; }

    /// <summary>
    ///     Gets or sets the conjugate gradient update
    ///     method to be used during optimization.
    /// </summary>
    public ConjugateGradientMethod Method { get; set; }

    /// <summary>
    ///     Gets the number of iterations performed
    ///     in the last call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" />.
    /// </summary>
    /// <value>
    ///     The number of iterations performed
    ///     in the previous optimization.
    /// </value>
    public int Iterations { get; private set; }

    /// <summary>
    ///     Gets the number of function evaluations performed
    ///     in the last call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" />.
    /// </summary>
    /// <value>
    ///     The number of evaluations performed
    ///     in the previous optimization.
    /// </value>
    public int Evaluations { get; private set; }

    /// <summary>
    ///     Gets the number of linear searches performed
    ///     in the last call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" />.
    /// </summary>
    public int Searches { get; private set; }

    /// <summary>
    ///     Get the exit code returned in the last call to the
    ///     <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()" /> or
    ///     <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" /> methods.
    /// </summary>
    public ConjugateGradientCode Status { get; private set; }

    /// <summary>
    ///     Occurs when progress is made during the optimization.
    /// </summary>
    public event EventHandler<OptimizationProgressEventArgs> Progress;

    /// <summary>
    ///     Called when the <see cref="IOptimizationMethod{TInput, TOutput}.NumberOfVariables" /> property has changed.
    /// </summary>
    /// <param name="numberOfVariables">The number of variables.</param>
    protected override void OnNumberOfVariablesChanged(int numberOfVariables)
    {
        base.OnNumberOfVariablesChanged(numberOfVariables);

        d = new double[numberOfVariables];
        gold = new double[numberOfVariables];
        w = new double[numberOfVariables];
    }

    /// <summary>
    ///     Implements the actual optimization algorithm. This
    ///     method should try to minimize the objective function.
    /// </summary>
    protected override bool Optimize()
    {
        // This code has been adapted from the original
        // FORTRAN function CGFAM by Jorge Nocedal, 1992.

        var irest = 1;
        var n = NumberOfVariables;
        var x = Solution;

        var f = Function(x);
        g = Gradient(x);

        var method = (int)Method;

        Iterations = 0;
        Evaluations = 1;
        var bnew = true;
        var nrst = 0;
        var im = 0; // Number of times betapr was negative for method 2 or 3
        Searches = 0; // Number of line search iterations after Wolfe conditions were satisfied.
        double dg0 = 0;

        for (var i = 0; i < g.Length; ++i)
            d[i] = -g[i];

        var gnorm = g.Euclidean();
        var xnorm = Math.Max(1.0, x.Euclidean());
        var stp1 = 1.0 / gnorm;
        var f_old = f;
        var finish = false;

        // Make initial progress report with initialization parameters
        if (Progress is not null)
            Progress(this, new OptimizationProgressEventArgs
                (Iterations, Evaluations, g, gnorm, Solution, xnorm, f, stp1, finish));
        // Main iteration
        while (!finish)
        {
            if (Token.IsCancellationRequested)
                break;

            Iterations++;
            nrst++;

            // Call the line search routine of Mor'e and Thuente
            // (modified for Nocedal's CG method) 
            // ------------------------------------------------- 
            //
            //  J.J. Mor'e and D. Thuente, "Linesearch Algorithms with Guaranteed 
            //  Sufficient Decrease". ACM Transactions on Mathematical 
            //  Software 20 (1994), pp 286-307. 
            //

            var nfev = 0;
            var info = 0;
            double dgout = 0;

            // Save original gradient
            for (var i = 0; i < g.Length; i++)
                gold[i] = g[i];

            var dg = d.Dot(g);
            var dgold = dg;
            var stp = 1.0;

            // Shanno-Phua's formula for trial step
            if (!bnew) stp = dg0 / dg;

            if (Iterations == 1)
                stp = stp1;

            var ides = 0;
            bnew = false;

        L72:

            // Call to the line search subroutine
            Status = cvsmod(ref f, d, ref stp, ref info, ref nfev, w, ref dg, ref dgout);

            if (Status != ConjugateGradientCode.Success)
                return false;

            // Test if descent direction is obtained for methods 2 and 3
            double gg = Matrix.Dot(g, g);
            double gg0 = Matrix.Dot(g, gold);
            var betapr = (gg - gg0) / (gnorm * gnorm);

            // When nrst > n and irest == 1 then restart.
            if (irest == 1 && nrst > n)
            {
                nrst = 0;
                bnew = true;
            }
            else
            {
                if (method != 1)
                {
                    var dg1 = -gg + betapr * dgout;

                    if (dg1 >= 0.0)
                    {
                        ides++;

                        if (ides > 5)
                        {
                            Status = ConjugateGradientCode.DescentNotObtained;
                            return false;
                        }

                        goto L72; // retry
                    }
                }
            }

            Evaluations += nfev;
            Searches += ides;

            // Determine correct beta value for method chosen
            var betafr = gg / (gnorm * gnorm);
            double beta = 0;

            if (nrst == 0)
            {
                beta = 0.0;
            }
            else
            {
                if (method == 1)
                    beta = betafr;
                else if (method == 2)
                    beta = betapr;
                else if (method == 3) beta = Math.Max(0.0, betapr);

                if ((method == 2 || method == 3) && betapr < 0.0) im++;
            }

            // Compute the new direction
            for (var i = 0; i < g.Length; i++)
                d[i] = -g[i] + beta * d[i];

            dg0 = dgold * stp;

            // Check for termination
            gnorm = g.Euclidean();
            xnorm = Math.Max(1.0, x.Euclidean());

            // Convergence test
            if (gnorm / xnorm <= epsilon)
                finish = true;

            // Stopping criteria by function delta
            if (Tolerance > 0 && Iterations > 1)
            {
                var delta = (f_old - f) / f;
                f_old = f;

                if (delta < Tolerance)
                    finish = true;
            }

            // Stopping criteria by max iterations
            if (MaxIterations > 0)
                if (Iterations > MaxIterations)
                    finish = true;

            if (Progress is not null)
                Progress(this, new OptimizationProgressEventArgs(Iterations,
                    Evaluations, g, gnorm, Solution, xnorm, f, stp, finish));
        }

        return Status == ConjugateGradientCode.Success;
    }

    private ConjugateGradientCode cvsmod(ref double f, double[] s, ref double stp, ref int info,
        ref int nfev, double[] wa, ref double dginit, ref double dgout)
    {
        var n = NumberOfVariables;

        var x = Solution;

        if (info == 1)
            goto L321;

        infoc = 1;

        if (stp <= 0) // Check the input parameters for errors
            return ConjugateGradientCode.StepSize;

        // Compute the initial gradient in the search direction
        // and check that S is a descent direction.

        if (dginit >= 0)
            throw new LineSearchFailedException(0, "The search direction is not a descent direction.");

        // Initialize local variables
        brackt = false;
        stage1 = true;
        nfev = 0;
        finit = f;
        dgtest = ftol * dginit;
        width = stpmax - stpmin;
        width1 = width / 0.5;

        for (var j = 0; j < x.Length; ++j)
            wa[j] = x[j];

        // The variables STX, FX, DGX contain the values of the step,
        //   function, and directional derivative at the best step.
        // The variables STY, FY, DGY contain the value of the step,
        //   function, and derivative at the other endpoint of the interval
        //   of uncertainty.
        // The variables STP, F, DG contain the values of the step,
        //   function, and derivative at the current step.

        stx = 0;
        fx = finit;
        dgx = dginit;
        sty = 0;
        fy = finit;
        dgy = dginit;
    L30: // Start of iteration.

        // Set the minimum and maximum steps to correspond
        // to the present interval of uncertainty.

        if (brackt)
        {
            stmin = Math.Min(stx, sty);
            stmax = Math.Max(stx, sty);
        }
        else
        {
            stmin = stx;
            stmax = stp + 4 * (stp - stx);
        }

        // Force the step to be within
        // the bounds STPMAX and STPMIN.

        stp = Math.Max(stp, stpmin);
        stp = Math.Min(stp, stpmax);

        // If an unusual termination is to occur then 
        // let STP be the lowest point obtained so far.

        if (brackt && (stp <= stmin || stp >= stmax) || nfev >= maxfev - 1 ||
            infoc == 0 || brackt && stmax - stmin <= xtol * stmax)
            stp = stx;

        // Evaluate the function and gradient at STP
        // and compute the directional derivative.

        for (var j = 0; j < s.Length; ++j)
            x[j] = wa[j] + stp * s[j];

        // Fetch function and gradient
        f = Function(x);
        g = Gradient(x);

        info = 0;
        nfev++;
        dg2 = 0;

        for (var j = 0; j < g.Length; ++j)
            dg2 += g[j] * s[j];

        ftest1 = finit + stp * dgtest;

        if (brackt && (stp <= stmin || stp >= stmax) || infoc == 0)
            return ConjugateGradientCode.RoundingErrors;

        if (stp == stpmax && f <= ftest1 && dg2 <= dgtest)
            return ConjugateGradientCode.StepHigh;

        if (stp == stpmin && (f > ftest1 || dg2 >= dgtest))
            return ConjugateGradientCode.StepLow;

        if (nfev >= maxfev)
            return ConjugateGradientCode.MaximumEvaluations;

        if (brackt && stmax - stmin <= xtol * stmax)
            return ConjugateGradientCode.Precision;
        // More's code has been modified so that at least one new 
        //  function value is computed during the line search (enforcing 
        //  at least one interpolation is not easy, since the code may 
        //  override an interpolation) 

        if (f <= ftest1 && Math.Abs(dg2) <= gtol * -dginit && nfev > 1)
        {
            info = 1;
            dgout = dg2;
            return ConjugateGradientCode.Success;
        }
    L321:

        // In the first stage we seek a step for which the modified
        // function has a nonpositive value and nonnegative derivative.
        if (stage1 && f <= ftest1 && dg2 >= Math.Min(ftol, gtol) * dginit) stage1 = false;

        // A modified function is used to predict the step only if
        // we have not obtained a step for which the modified function
        // has a nonpositive function value and nonnegative derivative,
        // and if a lower function value has been obtained but the 
        // decrease is not sufficient.

        if (stage1 && f <= fx && f > ftest1)
        {
            // Define the modified function and derivative values
            var fm = f - stp * dgtest;
            var fxm = fx - stx * dgtest;
            var fym = fy - sty * dgtest;
            var dgm = dg2 - dgtest;
            var dgxm = dgx - dgtest;
            var dgym = dgy - dgtest;

            // Call CSTEPM to update the interval of
            // uncertainty and to compute the new step.

            BoundedBroydenFletcherGoldfarbShanno.dcstep(ref stx, ref fxm, ref dgxm,
                ref sty, ref fym, ref dgym, ref stp, fm, dgm, ref brackt, stpmin, stpmax);

            // Reset the function and gradient values for f.
            fx = fxm + stx * dgtest;
            fy = fym + sty * dgtest;
            dgx = dgxm + dgtest;
            dgy = dgym + dgtest;
        }
        else
        {
            // Call CSTEPM to update the interval of
            // uncertainty and to compute the new step.
            BoundedBroydenFletcherGoldfarbShanno.dcstep(ref stx, ref fx, ref dgx,
                ref sty, ref fy, ref dgy, ref stp, f, dg2, ref brackt, stpmin, stpmax);
        }

        // Force a sufficient decrease in the 
        // size of the interval of uncertainty.

        if (brackt)
        {
            if (Math.Abs(sty - stx) >= 0.66 * width1)
                stp = stx + 0.5 * (sty - stx);

            width1 = width;
            width = Math.Abs(sty - stx);
        }

        goto L30;
    }
}