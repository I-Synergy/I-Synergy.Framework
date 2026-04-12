using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Statistics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Hinge loss.
/// </summary>
[Serializable]
public struct HingeLoss : ILoss<double[]>,
    IDifferentiableLoss<bool, double, double>,
    IDifferentiableLoss<double, double, double>
{
    /// <summary>
    ///     Gets or sets the expected outputs (the ground truth).
    /// </summary>
    public bool[][] Expected { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HingeLoss(double[][] expected)
    {
        Expected = Classes.Decide(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HingeLoss(double[] expected)
    {
        Expected = Classes.Decide(Jagged.ColumnVector(expected));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Jagged.OneHot<bool> is safe for AOT as bool is a known type.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Jagged.OneHot<bool> is safe for AOT as bool is a known type.")]
    public HingeLoss(int[] expected)
    {
        if (Classes.IsMinusOnePlusOne(expected))
            expected = expected.ToZeroOne();

        Expected = Jagged.OneHot<bool>(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HingeLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HingeLoss(bool[] expected)
    {
        Expected = Jagged.ColumnVector(expected);
    }

    /// <summary>
    ///     Computes the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(double[][] actual)
    {
        double error = 0;
        for (var i = 0; i < Expected.Length; i++)
            for (var j = 0; j < Expected[i].Length; j++)
                error += Loss(Expected[i][j], actual[i][j]);
        return error;
    }

    /// <summary>
    ///     Computes the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(double[] actual)
    {
        double error = 0;
        for (var i = 0; i < Expected.Length; i++)
            error += Loss(Expected[i][0], actual[i]);
        return error;
    }
    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(bool expected, double actual)
    {
        if (expected)
        {
            if (actual > 1)
                return 0;
            return 1 - actual;
        }

        if (actual < -1)
            return 0;
        return 1 + actual;
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Derivative(bool expected, double actual)
    {
        if (expected)
        {
            if (actual > 1)
                return 0;
            return actual;
        }

        if (actual < -1)
            return 0;
        return actual;
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(double expected, double actual)
    {
        return Loss(Classes.Decide(expected), actual);
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Derivative(double expected, double actual)
    {
        return Derivative(Classes.Decide(expected), actual);
    }
}