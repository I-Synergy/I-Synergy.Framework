using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Statistics;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Mean Accuracy loss, also known as zero-one-loss per
///     class. Equivalent to <see cref="ZeroOneLoss" /> but
///     for multi-label classifiers.
/// </summary>
[Serializable]
public class HammingLoss : LossBase<int[][]>,
    ILoss<bool[][]>, ILoss<double[][]>, ILoss<int[]>
{
    private bool mean = true;
    private int total;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HammingLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HammingLoss(double[][] expected)
    {
        Expected = expected.ToInt32();
        for (var i = 0; i < expected.Length; i++)
            total += expected[i].Length;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HammingLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HammingLoss(int[][] expected)
    {
        Expected = expected;
        for (var i = 0; i < expected.Length; i++)
            total += expected[i].Length;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HammingLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HammingLoss(int[] expected)
        : this(Jagged.OneHot(expected))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HammingLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public HammingLoss(bool[][] expected)
    {
        Expected = expected.ToInt32();
        for (var i = 0; i < expected.Length; i++)
            total += expected[i].Length;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the mean
    ///     accuracy loss should be computed. Default is true.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the mean accuracy loss should be computed; otherwise, <c>false</c>.
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
    public double Loss(bool[][] actual)
    {
        var error = 0;
        for (var i = 0; i < Expected.Length; i++)
            for (var j = 0; j < Expected[i].Length; j++)
                if (Classes.Decide(Expected[i][j]) != actual[i][j])
                    error++;

        if (mean)
            return error / (double)total;
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
    public double Loss(double[][] actual)
    {
        var error = 0;
        for (var i = 0; i < Expected.Length; i++)
            for (var j = 0; j < Expected[i].Length; j++)
                if (Classes.Decide(Expected[i][j]) != Classes.Decide(actual[i][j]))
                    error++;

        if (mean)
            return error / (double)total;
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
    public double Loss(int[] actual)
    {
        var error = 0;
        for (var i = 0; i < Expected.Length; i++)
            if (Expected[i][0] != actual[i])
                error++;

        if (mean)
            return error / (double)total;
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
    public override double Loss(int[][] actual)
    {
        var error = 0;
        for (var i = 0; i < Expected.Length; i++)
            for (var j = 0; j < Expected[i].Length; j++)
                if (Classes.Decide(Expected[i][j]) != Classes.Decide(actual[i][j]))
                    error++;

        if (mean)
            return error / (double)total;
        return error;
    }
}