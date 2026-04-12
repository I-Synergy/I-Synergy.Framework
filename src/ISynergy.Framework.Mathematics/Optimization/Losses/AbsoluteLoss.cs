using ISynergy.Framework.Mathematics.Common;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Absolute loss, also known as L1-loss.
/// </summary>
[Serializable]
public class AbsoluteLoss : LossBase<double[][]>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AbsoluteLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public AbsoluteLoss(double[][] expected)
    {
        Expected = expected;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the
    ///     mean absolute loss should be computed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the mean absolute loss should be computed; otherwise, <c>false</c>.
    /// </value>
    public bool Mean { get; set; }

    /// <summary>
    ///     Computes the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public override double Loss(double[][] actual)
    {
        double error = 0;
        for (var i = 0; i < Expected.Length; i++)
            error += Distance.Manhattan(Expected[i], actual[i]);

        if (Mean)
            error = error / Expected.Length;

        return error;
    }
}