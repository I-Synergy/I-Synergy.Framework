using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Statistics;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Logistic loss.
/// </summary>
[Serializable]
public struct LogisticLoss : ILoss<double[][]>, ILoss<double[]>,
    IDifferentiableLoss<bool, double, double>,
    IDifferentiableLoss<double, double, double>
{
    private double[][] expected;

    /// <summary>
    ///     Gets the expected outputs (the ground truth).
    /// </summary>
    public double[][] Expected
    {
        get => expected;
        set => expected = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LogisticLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public LogisticLoss(double[] expected)
    {
        this.expected = Jagged.ColumnVector(expected);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LogisticLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public LogisticLoss(double[][] expected)
    {
        this.expected = expected;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LogisticLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public LogisticLoss(int[] expected)
    {
        if (Classes.IsMinusOnePlusOne(expected))
            expected = expected.ToZeroOne();

        var oneHot = Jagged.OneHot<bool>(expected);
        this.expected = Classes.ToMinusOnePlusOne(oneHot);
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
            sum += Loss(Expected[i][0], actual[i]);
        return sum / Math.Log(2);
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
                sum += Loss(Expected[i][j], actual[i][j]);
        return sum / Math.Log(2);
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(double expected, double actual)
    {
        return Special.Log1p(Math.Exp(-expected * actual));
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Derivative(double expected, double actual)
    {
        return expected / (1 + Math.Exp(expected * actual));
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Loss(bool expected, double actual)
    {
        if (expected)
            return Special.Log1p(Math.Exp(-actual));
        return Special.Log1p(Math.Exp(actual));
    }

    /// <summary>
    ///     Computes the derivative of the loss between the expected values (ground truth)
    ///     and the given actual values that have been predicted.
    /// </summary>
    /// <param name="expected">The expected values that should have been predicted.</param>
    /// <param name="actual">The actual values that have been predicted.</param>
    /// <returns>
    ///     The loss value between the expected values and
    ///     the actual predicted values.
    /// </returns>
    public double Derivative(bool expected, double actual)
    {
        if (expected)
            return -1.0 / (1 + Math.Exp(actual));
        return 1.0 / (1 + Math.Exp(-actual));
    }
}