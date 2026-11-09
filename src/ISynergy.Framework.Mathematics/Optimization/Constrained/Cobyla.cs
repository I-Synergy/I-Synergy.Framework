
using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Optimization.Base;
using ISynergy.Framework.Mathematics.Optimization.Constrained.Constraints;



namespace ISynergy.Framework.Mathematics.Optimization.Constrained;

/// <summary>
///   Cobyla exit codes.
/// </summary>
/// 
public enum CobylaStatus
{
    /// <summary>
    ///   Optimization successfully completed.
    /// </summary>
    /// 
    Success = 0,

    /// <summary>
    ///   Maximum number of iterations (function/constraints evaluations) reached during optimization.
    /// </summary>
    /// 
    MaxIterationsReached,

    /// <summary>
    ///   Size of rounding error is becoming damaging, terminating prematurely.
    /// </summary>
    /// 
    DivergingRoundingErrors,

    /// <summary>
    ///   The posed constraints cannot be fulfilled.
    /// </summary>
    /// 
    NoPossibleSolution
}

/// <summary>
///   Constrained optimization by linear approximation.
/// </summary>
/// 
/// <remarks>
/// <para>
///   Constrained optimization by linear approximation (COBYLA) is a numerical 
///   optimization method for constrained problems where the derivative of the
///   objective function is not known, invented by Michael J. D. Powell. </para>
///
/// <para>
///   COBYLA2 is an implementation of Powell’s nonlinear derivative–free constrained
///   optimization that uses a linear approximation approach. The algorithm is a 
///   sequential trust–region algorithm that employs linear approximations to the
///   objective and constraint functions, where the approximations are formed by linear 
///   interpolation at n + 1 points in the space of the variables and tries to maintain
///   a regular–shaped simplex over iterations.</para>
///
/// <para>
///   This algorithm is able to solve non-smooth NLP problems with a moderate number
///   of variables (about 100), with inequality constraints only.</para>
///   
/// <para>    
///   References:
///   <list type="bullet">
///     <item><description><a href="http://en.wikipedia.org/wiki/COBYLA">
///       Wikipedia, The Free Encyclopedia. Cobyla. Available on:
///       http://en.wikipedia.org/wiki/COBYLA </a></description></item>
///   </list></para>
/// </remarks>
/// 
/// <example>
/// <para>
///   Let's say we would like to optimize a function whose gradient
///   we do not know or would is too difficult to compute. All we 
///   have to do is to specify the function, pass it to Cobyla and
///   call its Minimize() method:
/// </para>
/// 
/// <code>
/// // We would like to find the minimum of min f(x) = 10 * (x+1)^2 + y^2
/// Func&lt;double[], double> function = x => 10 * Math.Pow(x[0] + 1, 2) + Math.Pow(x[1], 2);
///   
/// // Create a cobyla method for 2 variables
/// Cobyla cobyla = new Cobyla(2, function);
/// 
/// bool success = cobyla.Minimize();
/// 
/// double minimum = minimum = cobyla.Value; // Minimum should be 0.
/// double[] solution = cobyla.Solution;     // Vector should be (-1, 0)
/// </code>
/// 
/// <para>
/// Cobyla can be used even when we have constraints in our optimization problem.
/// The following example can be found in Fletcher's book Practical Methods of
/// Optimization, under the equation number (9.1.15).
/// </para>
/// 
/// <code>
/// // We will optimize the 2-variable function f(x, y) = -x -y
/// var f = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);
/// 
/// // Under the following constraints
/// var constraints = new[]
/// {
///     new NonlinearConstraint(2, x =>             x[1] - x[0] * x[0] >= 0),
///     new NonlinearConstraint(2, x =>  1 - x[0] * x[0] - x[1] * x[1] >= 0),
/// };
///
/// // Create a Cobyla algorithm for the problem
/// var cobyla = new Cobyla(function, constraints);
///
/// // Optimize it
/// bool success = cobyla.Minimize();
/// double minimum = cobyla.Value;        // Minimum should be -2 * sqrt(0.5)
/// double[] solution = cobyla.Solution;  // Vector should be [sqrt(0.5), sqrt(0.5)]
/// </code>
/// </example>
///
public class Cobyla : BaseOptimizationMethod,
    IOptimizationMethod<CobylaStatus>
{

    readonly NonlinearConstraint[] constraints;
    /// <summary>
    ///   Gets the number of iterations performed in the last
    ///   call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/>.
    /// </summary>
    /// 
    /// <value>
    ///   The number of iterations performed
    ///   in the previous optimization.</value>
    ///   
    public int Iterations { get; private set; }

    /// <summary>
    ///   Gets or sets the value of RHOBEG. 
    ///   Default is 0.5.
    /// </summary>
    /// 
    public double RhoBeg { get; set; } = 0.5;

    /// <summary>
    ///   Gets or sets the value of RHOEND. 
    ///   Default is 1.0e-6.
    /// </summary>
    /// 
    public double RhoEnd { get; set; } = 1.0e-6;

    /// <summary>
    ///   Gets or sets the maximum number of iterations
    ///   to be performed during optimization. Default
    ///   is 0 (iterate until convergence).
    /// </summary>
    /// 
    public int MaxIterations { get; set; } = 3500;

    /// <summary>
    ///   Get the exit code returned in the last call to the
    ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()"/> or 
    ///   <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> methods.
    /// </summary>
    /// 
    public CobylaStatus Status { get; private set; }

    /// <summary>
    ///   Creates a new instance of the Cobyla optimization algorithm.
    /// </summary>
    /// 
    /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
    /// 
    public Cobyla(int numberOfVariables)
        : base(numberOfVariables)
    {
        constraints = Array.Empty<NonlinearConstraint>();
    }

    /// <summary>
    ///   Creates a new instance of the Cobyla optimization algorithm.
    /// </summary>
    /// 
    /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
    /// <param name="function">The function to be optimized.</param>
    /// 
    public Cobyla(int numberOfVariables, Func<double[], double> function)
        : base(numberOfVariables, function)
    {
        constraints = Array.Empty<NonlinearConstraint>();
    }

    /// <summary>
    ///   Creates a new instance of the Cobyla optimization algorithm.
    /// </summary>
    /// 
    /// <param name="function">The function to be optimized.</param>
    /// 
    public Cobyla(NonlinearObjectiveFunction function)
        : base(function.NumberOfVariables, function.Function)
    {
        constraints = Array.Empty<NonlinearConstraint>();
    }

    /// <summary>
    ///   Creates a new instance of the Cobyla optimization algorithm.
    /// </summary>
    /// 
    /// <param name="function">The function to be optimized.</param>
    /// <param name="constraints">The constraints of the optimization problem.</param>
    /// 
    public Cobyla(NonlinearObjectiveFunction function, IEnumerable<NonlinearConstraint> constraints)
        : this(function, System.Linq.Enumerable.ToArray(constraints))
    {
    }

    /// <summary>
    ///   Creates a new instance of the Cobyla optimization algorithm.
    /// </summary>
    /// 
    /// <param name="function">The function to be optimized.</param>
    /// <param name="constraints">The constraints of the optimization problem.</param>
    /// 
    public Cobyla(NonlinearObjectiveFunction function, NonlinearConstraint[] constraints)
        : base(function.NumberOfVariables, function.Function)
    {
        ArgumentNullException.ThrowIfNull(constraints);

        for (var i = 0; i < constraints.Length; i++)
        {
            if (constraints[i].NumberOfVariables != function.NumberOfVariables)
                throw new DimensionMismatchException(nameof(constraints), "The constraint at position " + i +
                    " contains an unexpected number of variables. Expected value would be " + NumberOfVariables + ".");

            if (constraints[i].ShouldBe == ConstraintType.EqualTo)
                throw new ArgumentException("Inequality constraints are not supported."
                    + " The constraint at position " + i + " is an inequality constraint.", nameof(constraints));
        }

        this.constraints = constraints;
    }
    /// <summary>
    ///   Implements the actual optimization algorithm. This
    ///   method should try to minimize the objective function.
    /// </summary>
    /// 
    protected override bool Optimize()
    {
        Status = cobyla();
        return Status == CobylaStatus.Success;
    }

    private CobylaStatus cobyla()
    {
        //     This subroutine minimizes an objective function F(X) subject to M
        //     inequality constraints on X, where X is a vector of variables that has
        //     N components.  The algorithm employs linear approximations to the
        //     objective and constraint functions, the approximations being formed by
        //     linear interpolation at N+1 points in the space of the variables.
        //     We regard these interpolation points as vertices of a simplex.  The
        //     parameter RHO controls the size of the simplex and it is reduced
        //     automatically from RHOBEG to RHOEND.  For each RHO the subroutine tries
        //     to achieve a good vector of variables for the current size, and then
        //     RHO is reduced until the value RHOEND is reached.  Therefore RHOBEG and
        //     RHOEND should be set to reasonable initial changes to and the required
        //     accuracy in the variables respectively, but this accuracy should be
        //     viewed as a subject for experimentation because it is not guaranteed.
        //     The subroutine has an advantage over many of its competitors, however,
        //     which is that it treats each constraint individually when calculating
        //     a change to the variables, instead of lumping the constraints together
        //     into a single penalty function.  The name of the subroutine is derived
        //     from the phrase Constrained Optimization BY Linear Approximations.

        //     The user must set the values of N, M, RHOBEG and RHOEND, and must
        //     provide an initial vector of variables in X.  Further, the value of
        //     IPRINT should be set to 0, 1, 2 or 3, which controls the amount of
        //     printing during the calculation. Specifically, there is no output if
        //     IPRINT=0 and there is output only at the end of the calculation if
        //     IPRINT=1.  Otherwise each new value of RHO and SIGMA is printed.
        //     Further, the vector of variables and some function information are
        //     given either when RHO is reduced or when each new value of F(X) is
        //     computed in the cases IPRINT=2 or IPRINT=3 respectively. Here SIGMA
        //     is a penalty parameter, it being assumed that a change to X is an
        //     improvement if it reduces the merit function
        //                F(X)+SIGMA*MAX(0.0, - C1(X), - C2(X),..., - CM(X)),
        //     where C1,C2,...,CM denote the constraint functions that should become
        //     nonnegative eventually, at least to the precision of RHOEND. In the
        //     printed output the displayed term that is multiplied by SIGMA is
        //     called MAXCV, which stands for 'MAXimum Constraint Violation'.  The
        //     argument ITERS is an integer variable that must be set by the user to a
        //     limit on the number of calls of CALCFC, the purpose of this routine being
        //     given below.  The value of ITERS will be altered to the number of calls
        //     of CALCFC that are made.

        //     In order to define the objective and constraint functions, we require
        //     a subroutine that has the name and arguments
        //                SUBROUTINE CALCFC (N,M,X,F,CON)
        //                DIMENSION X(:),CON(:)  .
        //     The values of N and M are fixed and have been defined already, while
        //     X is now the current vector of variables. The subroutine should return
        //     the objective and constraint functions at X in F and CON(1),CON(2),
        //     ...,CON(M).  Note that we are trying to adjust X so that F(X) is as
        //     small as possible subject to the constraint functions being nonnegative.
        // N.B. Arguments CON, SIM, SIMI, DATMAT, A, VSIG, VETA, SIGBAR, DX, W && IACT
        //      have been removed.

        //     Set the initial values of some parameters. The last column of SIM holds
        //     the optimal vertex of the current simplex, and the preceding N columns
        //     hold the displacements from the optimal vertex to the other vertices.
        //     Further, SIMI holds the inverse of the matrix that is contained in the
        //     first N columns of SIM.

        // Local variables

        int n = NumberOfVariables;
        int m = constraints.Length;
        int mpp = m + 2;

        double[] x = Solution;

        const double alpha = 0.25;
        const double beta = 2.1;
        const double gamma = 0.5;
        const double delta = 1.1;

        double f, resmax, total;

        var np = n + 1;
        var mp = m + 1;
        var rho = RhoBeg;
        var parmu = 0.0;

        var iflag = false;
        var ifull = false;
        var parsig = 0.0;
        var prerec = 0.0;
        var prerem = 0.0;

        var con = new double[1 + mpp];
        var sim = new double[1 + n, 1 + np];
        var simi = new double[1 + n, 1 + n];
        var datmat = new double[1 + mpp, 1 + np];
        var a = new double[1 + n, 1 + mp];
        var vsig = new double[1 + n];
        var veta = new double[1 + n];
        var sigbar = new double[1 + n];
        var dx = new double[1 + n];
        var w = new double[1 + n];

        Iterations = 0;
        var temp = 1.0 / rho;

        for (var i = 1; i <= n; ++i)
        {
            sim[i, np] = Solution[i - 1];
            sim[i, i] = rho;
            simi[i, i] = temp;
        }

        var jdrop = np;
        var ibrnch = false;

        CobylaStatus status = CobylaStatus.Success;

    //     Make the next call of the user-supplied subroutine CALCFC. These
    //     instructions are also used for calling CALCFC during the iterations of
    //     the algorithm.

    var skipPrelude = false;   // when set, jump straight to the iteration step (originally L_140)
    var pendingL550 = false;   // when set, run the RHO-reduction step (originally L_550) first

    while (true)
    {
        var toL550 = pendingL550;
        pendingL550 = false;

        if (!skipPrelude)
        {
            var exitPrelude = false;

            while (true)
            {
                if (Iterations >= MaxIterations && Iterations > 0)
                {
                    status = CobylaStatus.MaxIterationsReached;
                    exitPrelude = true;
                    break;
                }

                ++Iterations;

                // Compute objective function
                f = Function(x);

                // Compute constraints
                for (var i = 1; i <= m; i++)
                    con[i] = constraints[i - 1].GetViolation(x);

                resmax = 0.0;

                for (var k = 1; k <= m; ++k)
                    resmax = Math.Max(resmax, -con[k]);

                con[mp] = f;
                con[mpp] = resmax;

                if (ibrnch)
                {
                    //     Branch that decides whether x(*) should replace one of the
                    //     vertices of the current simplex (originally L_440).

                    var vmold = datmat[mp, np] + parmu * datmat[mpp, np];
                    var vmnew = f + parmu * resmax;
                    var trured = vmold - vmnew;

                    if (parmu == 0.0 && ExactComparisons.AreEqual(f, datmat[mp, np]))
                    {
                        prerem = prerec;
                        trured = datmat[mpp, np] - resmax;
                    }

                    var ratio = trured <= 0.0 ? 1.0 : 0.0;

                    jdrop = 0;

                    for (var j = 1; j <= n; ++j)
                    {
                        temp = 0.0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += simi[j, ii] * dx[ii];

                        temp = Math.Abs(temp);

                        if (temp > ratio)
                        {
                            jdrop = j;
                            ratio = temp;
                        }

                        sigbar[j] = temp * vsig[j];
                    }

                    var edgmax = delta * rho;
                    var l = 0;

                    for (var j = 1; j <= n; ++j)
                    {
                        if (sigbar[j] >= parsig || sigbar[j] >= vsig[j])
                        {
                            temp = veta[j];

                            if (trured > 0.0)
                            {
                                temp = 0.0;
                                for (var k = 1; k <= n; ++k)
                                    temp += Math.Pow(dx[k] - sim[k, j], 2.0);
                                temp = Math.Sqrt(temp);
                            }

                            if (temp > edgmax)
                            {
                                l = j;
                                edgmax = temp;
                            }
                        }
                    }

                    if (l > 0)
                        jdrop = l;

                    if (jdrop == 0)
                    {
                        toL550 = true;
                    }
                    else
                    {
                        //     Revise the simplex by updating the elements of SIM, SIMI and DATMAT.

                        temp = 0.0;
                        for (var i = 1; i <= n; ++i)
                        {
                            sim[i, jdrop] = dx[i];
                            temp += simi[jdrop, i] * dx[i];
                        }

                        for (var k = 1; k <= n; ++k)
                            simi[jdrop, k] /= temp;

                        for (var j = 1; j <= n; ++j)
                        {
                            if (j != jdrop)
                            {
                                temp = 0.0;
                                for (var ii = 1; ii <= n; ii++)
                                    temp += simi[j, ii] * dx[ii];

                                for (var k = 1; k <= n; ++k)
                                    simi[j, k] -= temp * simi[jdrop, k];
                            }
                        }

                        for (var k = 1; k <= mpp; ++k)
                            datmat[k, jdrop] = con[k];

                        if (trured > 0.0 && trured >= 0.1 * prerem)
                            toL550 = false;
                        else
                            toL550 = true;
                    }

                    break;
                }

                //     Set the recently calculated function values in a column of DATMAT.

                for (var i = 1; i <= mpp; ++i)
                    datmat[i, jdrop] = con[i];

                if (Iterations <= np)
                {
                    if (jdrop <= n)
                    {
                        if (datmat[mp, np] <= f)
                        {
                            x[jdrop - 1] = sim[jdrop, np];
                        }
                        else
                        {
                            sim[jdrop, np] = x[jdrop - 1];
                            for (var k = 1; k <= mpp; ++k)
                            {
                                datmat[k, jdrop] = datmat[k, np];
                                datmat[k, np] = con[k];
                            }
                            for (var k = 1; k <= jdrop; ++k)
                            {
                                sim[jdrop, k] = -rho;
                                temp = 0.0;
                                for (var i = k; i <= jdrop; ++i)
                                    temp -= simi[i, k];
                                simi[jdrop, k] = temp;
                            }
                        }
                    }

                    if (Iterations <= n)
                    {
                        jdrop = Iterations;
                        x[jdrop - 1] += rho;
                        continue;
                    }
                }

                ibrnch = true;
                toL550 = false;
                break;
            }

            if (exitPrelude)
                break;
        }

        if (toL550)
        {
            //     Otherwise reduce RHO if it is not at its least value and reset PARMU (originally L_550).

            if (!iflag)
            {
                ibrnch = false;
            }
            else if (rho > RhoEnd)
            {
                double cmin = 0.0, cmax = 0.0;

                rho *= 0.5;
                if (rho <= 1.5 * RhoEnd)
                    rho = RhoEnd;

                if (parmu > 0.0)
                {
                    var denom = 0.0;

                    for (var k = 1; k <= mp; ++k)
                    {
                        cmin = datmat[k, np];
                        cmax = cmin;

                        for (var i = 1; i <= n; ++i)
                        {
                            cmin = Math.Min(cmin, datmat[k, i]);
                            cmax = Math.Max(cmax, datmat[k, i]);
                        }

                        if (k <= m && cmin < 0.5 * cmax)
                        {
                            temp = Math.Max(cmax, 0.0) - cmin;
                            denom = denom <= 0.0 ? temp : Math.Min(denom, temp);
                        }
                    }

                    if (denom == 0.0)
                    {
                        parmu = 0.0;
                    }
                    else if (cmax - cmin < parmu * denom)
                    {
                        parmu = (cmax - cmin) / denom;
                    }
                }
            }
            else
            {
                status = CobylaStatus.Success;
                break;
            }
        }

        //     Iteration step (originally L_140).

        var jumpL40 = false;
        var jumpL550 = false;
        var retryL140 = false;

        do
        {
            retryL140 = false;

            var phimin = datmat[mp, np] + parmu * datmat[mpp, np];
            var nbest = np;

            for (var j = 1; j <= n; ++j)
            {
                temp = datmat[mp, j] + parmu * datmat[mpp, j];
                if (temp < phimin)
                {
                    nbest = j;
                    phimin = temp;
                }
                else if (ExactComparisons.AreEqual(temp, phimin) && parmu == 0.0 && datmat[mpp, j] < datmat[mpp, nbest])
                {
                    nbest = j;
                }
            }

            if (nbest <= n)
            {
                for (var i = 1; i <= mpp; ++i)
                {
                    temp = datmat[i, np];
                    datmat[i, np] = datmat[i, nbest];
                    datmat[i, nbest] = temp;
                }
                for (var i = 1; i <= n; ++i)
                {
                    temp = sim[i, nbest];
                    sim[i, nbest] = 0.0;
                    sim[i, np] += temp;

                    double tempa = 0.0;
                    for (var k = 1; k <= n; ++k)
                    {
                        sim[i, k] -= temp;
                        tempa -= simi[k, i];
                    }
                    simi[nbest, i] = tempa;
                }
            }

            double error = 0.0;
            for (var i = 1; i <= n; ++i)
            {
                for (var j = 1; j <= n; ++j)
                {
                    temp = 0.0;
                    for (var ii = 1; ii <= n; ii++)
                        temp += simi[i, ii] * sim[ii, j];

                    if (i == j)
                        temp -= 1.0;
                    error = Math.Max(error, Math.Abs(temp));
                }
            }

            if (error > 0.1)
            {
                status = CobylaStatus.DivergingRoundingErrors;
                break;
            }

            for (var k = 1; k <= mp; ++k)
            {
                con[k] = -datmat[k, np];
                for (var j = 1; j <= n; ++j)
                    w[j] = datmat[k, j] + con[k];

                for (var i = 1; i <= n; ++i)
                {
                    temp = 0.0;
                    for (var ii = 1; ii <= n; ii++)
                        temp += w[ii] * simi[ii, i];

                    a[i, k] = (k == mp ? -1.0 : 1.0) * temp;
                }
            }

            iflag = true;
            parsig = alpha * rho;
            var pareta = beta * rho;

            for (var j = 1; j <= n; ++j)
            {
                var wsig = 0.0;
                for (var k = 1; k <= n; ++k)
                    wsig += simi[j, k] * simi[j, k];

                var weta = 0.0;
                for (var k = 1; k <= n; ++k)
                    weta += sim[k, j] * sim[k, j];

                vsig[j] = 1.0 / Math.Sqrt(wsig);
                veta[j] = Math.Sqrt(weta);

                if (vsig[j] < parsig || veta[j] > pareta)
                    iflag = false;
            }

            if (!ibrnch && !iflag)
            {
                jdrop = 0;
                temp = pareta;
                for (var j = 1; j <= n; ++j)
                {
                    if (veta[j] > temp)
                    {
                        jdrop = j;
                        temp = veta[j];
                    }
                }
                if (jdrop == 0)
                {
                    for (var j = 1; j <= n; ++j)
                    {
                        if (vsig[j] < temp)
                        {
                            jdrop = j;
                            temp = vsig[j];
                        }
                    }
                }

                temp = gamma * rho * vsig[jdrop];
                for (var k = 1; k <= n; ++k)
                    dx[k] = temp * simi[jdrop, k];
                var cvmaxp = 0.0;
                var cvmaxm = 0.0;

                total = 0.0;
                for (var k = 1; k <= mp; ++k)
                {
                    total = 0.0;
                    for (var ii = 1; ii <= n; ii++)
                        total += a[ii, k] * dx[ii];

                    if (k < mp)
                    {
                        temp = datmat[k, np];
                        cvmaxp = Math.Max(cvmaxp, -total - temp);
                        cvmaxm = Math.Max(cvmaxm, total - temp);
                    }
                }

                var dxsign = parmu * (cvmaxp - cvmaxm) > 2.0 * total ? -1.0 : 1.0;

                temp = 0.0;
                for (var i = 1; i <= n; ++i)
                {
                    dx[i] = dxsign * dx[i];
                    sim[i, jdrop] = dx[i];
                    temp += simi[jdrop, i] * dx[i];
                }

                for (var k = 1; k <= n; ++k)
                    simi[jdrop, k] /= temp;

                for (var j = 1; j <= n; ++j)
                {
                    if (j != jdrop)
                    {
                        temp = 0.0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += simi[j, ii] * dx[ii];

                        for (var k = 1; k <= n; ++k)
                            simi[j, k] -= temp * simi[jdrop, k];
                    }

                    x[j - 1] = sim[j, np] + dx[j];
                }

                jumpL40 = true;
                break;
            }

            trstlp(n, m, a, con, rho, dx, out ifull);

            if (!ifull)
            {
                temp = 0.0;
                for (var k = 1; k <= n; ++k)
                    temp += dx[k] * dx[k];

                if (temp < 0.25 * rho * rho)
                {
                    ibrnch = true;
                    jumpL550 = true;
                    break;
                }
            }

            total = 0.0;
            var resnew = 0.0;
            con[mp] = 0.0;

            for (var k = 1; k <= mp; ++k)
            {
                total = 0.0;
                for (var ii = 1; ii <= n; ii++)
                    total += a[ii, k] * dx[ii];

                total = con[k] - total;

                if (k < mp)
                    resnew = Math.Max(resnew, total);
            }

            prerec = datmat[mpp, np] - resnew;
            var barmu = prerec > 0.0 ? total / prerec : 0.0;

            if (parmu < 1.5 * barmu)
            {
                parmu = 2.0 * barmu;

                var phi = datmat[mp, np] + parmu * datmat[mpp, np];

                for (var j = 1; j <= n; ++j)
                {
                    temp = datmat[mp, j] + parmu * datmat[mpp, j];
                    if (temp < phi ||
                        (ExactComparisons.AreEqual(temp, phi) && parmu == 0.0 && datmat[mpp, j] < datmat[mpp, np]))
                    {
                        retryL140 = true;
                        break;
                    }
                }

                if (retryL140)
                    continue;
            }

            prerem = parmu * prerec - total;

            for (var k = 1; k <= n; ++k)
                x[k - 1] = sim[k, np] + dx[k];

            ibrnch = true;
            jumpL40 = true;
            break;
        }
        while (retryL140);

        if (jumpL40)
        {
            skipPrelude = false;
            continue;
        }

        if (jumpL550)
        {
            skipPrelude = true;
            pendingL550 = true;
            continue;
        }

        // The rounding-errors path broke out of the do-loop after setting the status.
        break;
    }

    //     Return the best calculated values of the variables (originally L_600 / L_620).

    if (!(status == CobylaStatus.Success && ifull))
    {
        for (var k = 1; k <= n; ++k)
            x[k - 1] = sim[k, np];
    }

    MaxIterations = Iterations;

    if (status == CobylaStatus.Success)
    {
        // Check if all constraints have been fulfilled
        for (var i = 0; i < constraints.Length; i++)
            if (constraints[i].IsViolated(x))
                status = CobylaStatus.NoPossibleSolution;
    }

        return status;
    }

    private static void trstlp(int n, int m, double[,] a, double[] b,
        double rho, double[] dx, out bool ifull)
    {
        // N.B. Arguments Z, ZDOTA, VMULTC, SDIRN, DXNEW, VMULTD && IACT have been removed.

        //     This subroutine calculates an N-component vector DX by applying the
        //     following two stages. In the first stage, DX is set to the shortest
        //     vector that minimizes the greatest violation of the constraints
        //     subject to the Euclidean length of DX being at most RHO. If its length is
        //     strictly less than RHO, then we use the resultant freedom in DX to
        //     minimize the objective function subject to no increase in any greatest
        //     constraint violation.
        //
        //     In general NACT is the number of constraints in the active set and
        //     IACT(1),...,IACT(NACT) are their indices, while the remainder of IACT
        //     contains a permutation of the remaining constraint indices.

        // Local variables

        ifull = true;

        double temp;

        var nactx = 0;
        var resold = 0.0;

        var z = new double[1 + n, 1 + n];
        var zdota = new double[2 + m];
        var vmultc = new double[2 + m];
        var sdirn = new double[1 + n];
        var dxnew = new double[1 + n];
        var vmultd = new double[2 + m];
        var iact = new int[2 + m];

        var mcon = m;
        var nact = 0;

        for (var i = 1; i <= n; ++i)
        {
            z[i, i] = 1.0;
            dx[i] = 0.0;
        }

        var icon = 0;
        var resmax = 0.0;

        if (m >= 1)
        {
            for (var k = 1; k <= m; ++k)
            {
                if (b[k] > resmax)
                {
                    resmax = b[k];
                    icon = k;
                }
            }
            for (var k = 1; k <= m; ++k)
            {
                iact[k] = k;
                vmultc[k] = resmax - b[k];
            }
        }

        // When there is no constraint violation to resolve, switch to stage two
        // before starting the main iteration (originally L_480 reached from the top).

        if (resmax == 0.0)
        {
            mcon = m + 1;
            icon = mcon;
            iact[mcon] = mcon;
            vmultc[mcon] = 0.0;
        }

        // Outer stage loop (originally L_60).
        while (true)
        {
            var optold = 0.0;
            var icount = 0;

            var toL490 = false;

            // Inner iteration loop (originally L_70).
            while (true)
            {
                double optnew;

                if (mcon == m)
                {
                    optnew = resmax;
                }
                else
                {
                    temp = 0.0;
                    for (var ii = 1; ii <= n; ii++)
                        temp += dx[ii] * a[ii, mcon];

                    optnew = -temp;
                }

                if (icount == 0 || optnew < optold)
                {
                    optold = optnew;
                    nactx = nact;
                    icount = 3;
                }
                else if (nact > nactx)
                {
                    nactx = nact;
                    icount = 3;
                }
                else
                {
                    --icount;
                }

                if (icount == 0)
                {
                    toL490 = true;
                    break;
                }

                var kk = 0;
                var jumpL320 = false;

                if (icon <= nact)
                {
                    // Delete the constraint that has the index IACT(ICON) from the active set (L_260).

                    if (icon < nact)
                    {
                        var isave = iact[icon];
                        var vsave = vmultc[icon];
                        int k = icon;

                        do
                        {
                            var kp = k + 1;
                            kk = iact[kp];
                            double sp = 0;
                            for (var ii = 1; ii <= n; ii++)
                                sp += z[ii, k] * a[ii, kk];

                            temp = Math.Sqrt(sp * sp + zdota[kp] * zdota[kp]);

                            var alpha = zdota[kp] / temp;
                            var beta = sp / temp;
                            zdota[kp] = alpha * zdota[k];
                            zdota[k] = temp;

                            for (var i = 1; i <= n; ++i)
                            {
                                temp = alpha * z[i, kp] + beta * z[i, k];
                                z[i, kp] = alpha * z[i, k] - beta * z[i, kp];
                                z[i, k] = temp;
                            }

                            iact[k] = kk;
                            vmultc[k] = vmultc[kp];
                            k = kp;
                        } while (k < nact);

                        iact[k] = isave;
                        vmultc[k] = vsave;
                    }
                    --nact;

                    if (mcon > m)
                    {
                        jumpL320 = true;
                    }
                    else
                    {
                        temp = 0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += sdirn[ii] * z[ii, nact + 1];

                        for (var k = 1; k <= n; ++k)
                            sdirn[k] -= temp * z[k, nact + 1];
                    }
                }
                else
                {
                    // If ICON exceeds NACT, add the constraint with index IACT(ICON) to the
                    // active set. Apply Givens rotations so that the last N-NACT-1 columns
                    // of Z are orthogonal to the gradient of the new constraint.

                    kk = iact[icon];

                    for (var k = 1; k <= n; ++k)
                        dxnew[k] = a[k, kk];
                    var tot = 0.0;
                    int k__r1 = n;
                    while (k__r1 > nact)
                    {
                        var sp__r2 = 0.0;
                        var spabs__r3 = 0.0;
                        for (var i__r4 = 1; i__r4 <= n; ++i__r4)
                        {
                            temp = z[i__r4, k__r1] * dxnew[i__r4];
                            sp__r2 += temp;
                            spabs__r3 += Math.Abs(temp);
                        }
                        var acca__r5 = spabs__r3 + 0.1 * Math.Abs(sp__r2);
                        var accb__r6 = spabs__r3 + 0.2 * Math.Abs(sp__r2);
                        if (spabs__r3 >= acca__r5 || acca__r5 >= accb__r6) sp__r2 = 0.0;

                        if (tot == 0.0)
                        {
                            tot = sp__r2;
                        }
                        else
                        {
                            var kp__r7 = k__r1 + 1;
                            temp = Math.Sqrt(sp__r2 * sp__r2 + tot * tot);
                            var alpha__r8 = sp__r2 / temp;
                            var beta__r9 = tot / temp;
                            tot = temp;
                            for (var i__r4 = 1; i__r4 <= n; ++i__r4)
                            {
                                temp = alpha__r8 * z[i__r4, k__r1] + beta__r9 * z[i__r4, kp__r7];
                                z[i__r4, kp__r7] = alpha__r8 * z[i__r4, kp__r7] - beta__r9 * z[i__r4, k__r1];
                                z[i__r4, k__r1] = temp;
                            }
                        }
                        --k__r1;
                    }

                    if (tot != 0.0)
                    {
                        // Add the new constraint if this can be done without a deletion.
                        ++nact;
                        zdota[nact] = tot;
                        vmultc[icon] = vmultc[nact];
                        vmultc[nact] = 0.0;
                    }
                    else
                    {
                        // A deletion has to be made. Set VMULTD and find IOUT.
                        var ratioDel = -1.0;
                        int k__r10 = nact;
                        do
                        {
                            var zdotv__r11 = 0.0;
                            var zdvabs__r12 = 0.0;

                            for (var i__r13 = 1; i__r13 <= n; ++i__r13)
                            {
                                temp = z[i__r13, k__r10] * dxnew[i__r13];
                                zdotv__r11 = zdotv__r11 + temp;
                                zdvabs__r12 = zdvabs__r12 + Math.Abs(temp);
                            }

                            var acca__r14 = zdvabs__r12 + 0.1 * Math.Abs(zdotv__r11);
                            var accb__r15 = zdvabs__r12 + 0.2 * Math.Abs(zdotv__r11);

                            if (zdvabs__r12 < acca__r14 && acca__r14 < accb__r15)
                            {
                                temp = zdotv__r11 / zdota[k__r10];

                                if (temp > 0.0 && iact[k__r10] <= m)
                                {
                                    var tempa__r16 = vmultc[k__r10] / temp;
                                    if (ratioDel < 0.0 || tempa__r16 < ratioDel) ratioDel = tempa__r16;
                                }

                                if (k__r10 >= 2)
                                {
                                    var kw__r17 = iact[k__r10];
                                    for (var i__r13 = 1; i__r13 <= n; ++i__r13) dxnew[i__r13] -= temp * a[i__r13, kw__r17];
                                }
                                vmultd[k__r10] = temp;
                            }
                            else
                            {
                                vmultd[k__r10] = 0.0;
                            }
                        } while (--k__r10 > 0);

                        if (ratioDel < 0.0)
                        {
                            toL490 = true;
                            break;
                        }

                        // Revise the Lagrange multipliers and reorder the active constraints.
                        for (var k = 1; k <= nact; ++k)
                            vmultc[k] = Math.Max(0.0, vmultc[k] - ratioDel * vmultd[k]);

                        if (icon < nact)
                        {
                            var isave = iact[icon];
                            var vsave = vmultc[icon];
                            int k = icon;
                            do
                            {
                                var kp = k + 1;
                                var kw = iact[kp];

                                double sp = 0.0;
                                for (var ii = 1; ii <= n; ii++)
                                    sp += z[ii, k] * a[ii, kw];

                                temp = Math.Sqrt(sp * sp + zdota[kp] * zdota[kp]);

                                var alpha = zdota[kp] / temp;
                                var beta = sp / temp;
                                zdota[kp] = alpha * zdota[k];
                                zdota[k] = temp;

                                for (var i = 1; i <= n; ++i)
                                {
                                    temp = alpha * z[i, kp] + beta * z[i, k];
                                    z[i, kp] = alpha * z[i, k] - beta * z[i, kp];
                                    z[i, k] = temp;
                                }
                                iact[k] = kw;
                                vmultc[k] = vmultc[kp];
                                k = kp;
                            } while (k < nact);

                            iact[k] = isave;
                            vmultc[k] = vsave;
                        }

                        temp = 0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += z[ii, nact] * a[ii, kk];

                        if (temp == 0.0)
                        {
                            toL490 = true;
                            break;
                        }

                        zdota[nact] = temp;
                        vmultc[icon] = 0.0;
                        vmultc[nact] = ratioDel;
                    }

                    // Update IACT and ensure that the objective function continues to be
                    // treated as the last active constraint when MCON>M (originally L_210).

                    iact[icon] = iact[nact];
                    iact[nact] = kk;

                    if (mcon > m && kk != mcon)
                    {
                        int k = nact - 1;

                        double sp = 0;
                        for (var ii = 1; ii <= n; ii++)
                            sp += z[ii, k] * a[ii, kk];

                        temp = Math.Sqrt(sp * sp + zdota[nact] * zdota[nact]);

                        var alpha = zdota[nact] / temp;
                        var beta = sp / temp;
                        zdota[nact] = alpha * zdota[k];
                        zdota[k] = temp;

                        for (var i = 1; i <= n; ++i)
                        {
                            temp = alpha * z[i, nact] + beta * z[i, k];
                            z[i, nact] = alpha * z[i, k] - beta * z[i, nact];
                            z[i, k] = temp;
                        }

                        iact[nact] = iact[k];
                        iact[k] = kk;
                        temp = vmultc[k];
                        vmultc[k] = vmultc[nact];
                        vmultc[nact] = temp;
                    }

                    if (mcon > m)
                    {
                        jumpL320 = true;
                    }
                    else
                    {
                        kk = iact[nact];

                        temp = 0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += sdirn[ii] * a[ii, kk];
                        temp = (temp - 1.0) / zdota[nact];

                        for (var k = 1; k <= n; ++k)
                            sdirn[k] -= temp * z[k, nact];
                    }
                }

                // Pick the next search direction of stage two (originally L_320).
                if (jumpL320)
                {
                    temp = 1.0 / zdota[nact];

                    for (var k = 1; k <= n; ++k)
                        sdirn[k] = temp * z[k, nact];
                }

                // Calculate the step to the boundary of the trust region (originally L_340).

                var dd = rho * rho;
                var sd = 0.0;
                var ss = 0.0;

                for (var i = 1; i <= n; ++i)
                {
                    if (Math.Abs(dx[i]) >= 1.0E-6 * rho)
                        dd -= dx[i] * dx[i];

                    sd += dx[i] * sdirn[i];
                    ss += sdirn[i] * sdirn[i];
                }

                if (dd <= 0.0)
                {
                    toL490 = true;
                    break;
                }

                temp = Math.Sqrt(ss * dd);

                if (Math.Abs(sd) >= 1.0E-6 * temp)
                    temp = Math.Sqrt(ss * dd + sd * sd);

                var stpful = dd / (temp + sd);
                var step = stpful;

                if (mcon == m)
                {
                    var acca = step + 0.1 * resmax;
                    var accb = step + 0.2 * resmax;

                    if (step >= acca || acca >= accb)
                    {
                        // Switch to stage two (originally L_480).
                        break;
                    }

                    step = Math.Min(step, resmax);
                }

                // Set DXNEW to the new variables if STEP is the steplength.
                for (var k = 1; k <= n; ++k)
                    dxnew[k] = dx[k] + step * sdirn[k];

                if (mcon == m)
                {
                    resold = resmax;
                    resmax = 0.0;
                    for (var k = 1; k <= nact; ++k)
                    {
                        kk = iact[k];

                        temp = 0;
                        for (var ii = 1; ii <= n; ii++)
                            temp += a[ii, kk] * dxnew[ii];

                        temp = b[kk] - temp;
                        resmax = Math.Max(resmax, temp);
                    }
                }

                // Complete the multipliers below the active set (originally L_390).
                var k__r18 = nact;
                while (true)
                {
                    var zdotw__r19 = 0.0;
                    var zdwabs__r20 = 0.0;
                    for (var i__r21 = 1; i__r21 <= n; ++i__r21)
                    {
                        temp = z[i__r21, k__r18] * dxnew[i__r21];
                        zdotw__r19 += temp;
                        zdwabs__r20 += Math.Abs(temp);
                    }

                    var acca__r22 = zdwabs__r20 + 0.1 * Math.Abs(zdotw__r19);
                    var accb__r23 = zdwabs__r20 + 0.2 * Math.Abs(zdotw__r19);

                    if (zdwabs__r20 >= acca__r22 || acca__r22 >= accb__r23)
                        zdotw__r19 = 0.0;

                    vmultd[k__r18] = zdotw__r19 / zdota[k__r18];

                    if (k__r18 < 2)
                        break;

                    kk = iact[k__r18];
                    for (var i__r21 = 1; i__r21 <= n; ++i__r21)
                        dxnew[i__r21] -= vmultd[k__r18] * a[i__r21, kk];
                    --k__r18;
                }

                if (mcon > m)
                    vmultd[nact] = Math.Max(0.0, vmultd[nact]);

                // Complete VMULTC by finding the new constraint residuals.
                for (var k = 1; k <= n; ++k)
                    dxnew[k] = dx[k] + step * sdirn[k];

                if (mcon > nact)
                {
                    var kl = nact + 1;

                    for (var k = kl; k <= mcon; ++k)
                    {
                        kk = iact[k];
                        var total = resmax - b[kk];
                        var sumabs = resmax + Math.Abs(b[kk]);

                        for (var i = 1; i <= n; ++i)
                        {
                            temp = a[i, kk] * dxnew[i];
                            total += temp;
                            sumabs += Math.Abs(temp);
                        }

                        var acca = sumabs + 0.1 * Math.Abs(total);
                        var accb = sumabs + 0.2 * Math.Abs(total);

                        if (sumabs >= acca || acca >= accb)
                            total = 0.0;

                        vmultd[k] = total;
                    }
                }

                // Calculate the fraction of the step from DX to DXNEW that will be taken.
                var ratio = 1.0;
                icon = 0;
                for (var k = 1; k <= mcon; ++k)
                {
                    if (vmultd[k] < 0.0)
                    {
                        temp = vmultc[k] / (vmultc[k] - vmultd[k]);
                        if (temp < ratio)
                        {
                            ratio = temp;
                            icon = k;
                        }
                    }
                }

                // Update DX, VMULTC and RESMAX.
                temp = 1.0 - ratio;
                for (var k = 1; k <= n; ++k)
                    dx[k] = temp * dx[k] + ratio * dxnew[k];

                for (var k = 1; k <= mcon; ++k)
                    vmultc[k] = Math.Max(0.0, temp * vmultc[k] + ratio * vmultd[k]);

                if (mcon == m)
                    resmax = resold + ratio * (resmax - resold);

                if (icon > 0)
                    continue;   // begin another iteration (originally L_70)

                if (ExactComparisons.AreEqual(step, stpful))
                    return;

                // Switch to stage two (originally L_480).
                break;
            }

            // After the inner loop: either finish (originally L_490) or switch stage (L_480).
            if (toL490 && mcon != m)
            {
                ifull = false;
                return;
            }

            // Switch to stage two (originally L_480).
            mcon = m + 1;
            icon = mcon;
            iact[mcon] = mcon;
            vmultc[mcon] = 0.0;
        }
    }

}
