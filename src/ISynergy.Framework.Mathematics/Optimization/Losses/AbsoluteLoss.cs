namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Absolute loss, also known as L1-loss.
/// </summary>
[Serializable]
public class AbsoluteLoss : LossBase<double[][]>
{
    private bool mean;

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
    public bool Mean
    {
        get => mean;
        set => mean = value;
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
    public override double Loss(double[][] actual)
    {
        double error = 0;
        for (var i = 0; i < Expected.Length; i++)
            error += Distance.Manhattan(Expected[i], actual[i]);

        if (mean)
            error = error / Expected.Length;

        return error;
    }
}