using ISynergy.Framework.Mathematics.Statistics;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Categorical cross-entropy loss for multi-class problems,
///     also known as the logistic loss for softmax (categorical) outputs.
/// </summary>
/// <seealso cref="BinaryCrossEntropyLoss" />
[Serializable]
public class CategoryCrossEntropyLoss : LossBase<bool[][]>, ILoss<int[]>, ILoss<double[][]>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CategoryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public CategoryCrossEntropyLoss(double[][] expected)
    {
        Expected = Classes.Decide(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CategoryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public CategoryCrossEntropyLoss(int[] expected)
    {
        Expected = Jagged.OneHot<bool>(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CategoryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public CategoryCrossEntropyLoss(bool[][] expected)
    {
        Expected = expected;
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
        double sum = 0;
        for (var i = 0; i < actual.Length; i++)
            for (var j = 0; j < actual[i].Length; j++)
                sum -= Measures.Entropy(Expected[i][j], actual[i][j]);
        return sum;
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
        return Loss(Jagged.OneHot<bool>(actual));
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
    public override double Loss(bool[][] actual)
    {
        double sum = 0;
        for (var i = 0; i < actual.Length; i++)
            for (var j = 0; j < actual[i].Length; j++)
                sum -= Measures.Entropy(Expected[i][j], actual[i][j]);
        return sum;
    }
}