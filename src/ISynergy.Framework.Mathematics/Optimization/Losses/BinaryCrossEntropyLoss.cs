using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Statistics;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Binary cross-entropy loss for multi-label problems, also
///     known as logistic loss per output of a multi-label classifier.
/// </summary>
/// <seealso cref="CategoryCrossEntropyLoss" />
[Serializable]
public class BinaryCrossEntropyLoss : LossBase<bool[][]>,
    ILoss<int[]>, ILoss<double[]>, ILoss<double[][]>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public BinaryCrossEntropyLoss(int[][] expected)
    {
        Expected = Classes.Decide(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public BinaryCrossEntropyLoss(double[][] expected)
    {
        Expected = Classes.Decide(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public BinaryCrossEntropyLoss(double[] expected)
    {
        Expected = Jagged.ColumnVector(Classes.Decide(expected));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public BinaryCrossEntropyLoss(bool[] expected)
    {
        Expected = Jagged.ColumnVector(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryCrossEntropyLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public BinaryCrossEntropyLoss(int[] expected)
    {
        Expected = Jagged.OneHot<bool>(expected);
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
                if (Expected[i][j])
                    sum -= Math.Log(actual[i][j]);
                else
                    sum -= Special.Log1m(actual[i][j]);

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
    public double Loss(double[] actual)
    {
        double sum = 0;
        for (var i = 0; i < actual.Length; i++)
            if (Expected[i][0])
                sum -= Math.Log(actual[i]);
            else
                sum -= Special.Log1m(actual[i]);

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
        return Loss(Jagged.OneHot(actual));
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
        var sum = 0;
        for (var i = 0; i < actual.Length; i++)
            for (var j = 0; j < actual[i].Length; j++)
            {
                var e = Expected[i][j];
                var a = actual[i][j];

                if (e)
                {
                    if (!a)
                        sum--;
                }
                else
                {
                    if (a)
                        sum--;
                }
            }

        return sum;
    }
}