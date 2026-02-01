using ISynergy.Framework.Mathematics.Optimization.Constrained;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using System.Runtime.CompilerServices;
namespace ISynergy.Framework.Mathematics.Optimization;

/// <summary>
///   Contains classes for constrained and unconstrained optimization. Includes 
///   <see cref="ConjugateGradient">Conjugate Gradient (CG)</see>, <see cref="BoundedBroydenFletcherGoldfarbShanno">
///   Bounded</see> and <see cref="BroydenFletcherGoldfarbShanno">Unbounded Broyden–Fletcher–Goldfarb–Shanno (BFGS)</see>,
///   gradient-free optimization methods such as <see cref="Cobyla"/> and the <see cref="GoldfarbIdnani">Goldfarb-Idnani
///   </see> solver for Quadratic Programming (QP) problems.
/// </summary>
/// 
/// <remarks>
/// <para>
///   This namespace contains different methods for solving both constrained and unconstrained
///   optimization problems. For unconstrained optimization, methods available include 
///   <see cref="ConjugateGradient">Conjugate Gradient (CG)</see>, <see cref="BoundedBroydenFletcherGoldfarbShanno">
///   Bounded</see> and <see cref="BroydenFletcherGoldfarbShanno">Unbounded Broyden–Fletcher–Goldfarb–Shanno (BFGS)</see>,
///   <see cref="ResilientBackpropagation">Resilient Backpropagation</see> and a simplified implementation of the 
///   <see cref="TrustRegionNewtonMethod">Trust Region Newton _method (TRON)</see>.</para>
///   
/// <para>
///   For constrained optimization problems, methods available include the <see cref="AugmentedLagrangian">
///   Augmented Lagrangian method</see> for general non-linear optimization, <see cref="Cobyla"/> for
///   gradient-free non-linear optimization, and the <see cref="GoldfarbIdnani">Goldfarb-Idnani</see>
///   method for solving Quadratic Programming (QP) problems.</para>
///   
/// <para>
///   This namespace also contains optimizers specialized for least squares problems, such as <see cref="GaussNewton">
///   Gauss Newton</see> and the <see cref="LevenbergMarquardt">Levenberg-Marquart</see> least squares solvers.</para>
///   
/// <para>
///   For univariate problems, standard search algorithms are also available, such as <see cref="BrentSearch">
///   Brent</see> and <see cref="BinarySearch">Binary search</see>.</para>
///  
/// <para>
///   The namespace class diagram is shown below. </para>
///   <img src="..\diagrams\classes\ISynergy.Framework.Mathematics.Optimization.png" />
/// </remarks>
/// 
/// <seealso cref="ISynergy.Framework.Mathematics"/>
/// <seealso cref="ISynergy.Framework.Mathematics.Differentiation"/>
/// <seealso cref="ISynergy.Framework.Mathematics.Integration"/>
///   
[CompilerGenerated]
class NamespaceDoc
{
}
