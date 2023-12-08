namespace ISynergy.Framework.Mathematics.Optimization.Losses;

/// <summary>
///     Euclidean loss, also known as zero-one-loss. This class
///     provides exactly the same functionality as <see cref="SquareLoss" />
///     but has a more intuitive name. Both classes are interchangeable.
/// </summary>
[Serializable]
public class EuclideanLoss : SquareLoss
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EuclideanLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public EuclideanLoss(double[][] expected)
        : base(expected)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EuclideanLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public EuclideanLoss(double[] expected)
        : base(expected)
    {
    }
}

/// <summary>
///     Square loss, also known as L2-loss or Euclidean loss.
/// </summary>
[Serializable]
public class SquareLoss : LossBase<double[][]>
{
    private bool mean = true;
    private bool root;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SquareLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public SquareLoss(double[][] expected)
    {
        Expected = expected;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SquareLoss" /> class.
    /// </summary>
    /// <param name="expected">The expected outputs (ground truth).</param>
    public SquareLoss(double[] expected)
    {
        Expected = Jagged.ColumnVector(expected);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the
    ///     root square loss should be computed. If <see cref="Mean" />
    ///     is also set, computes the root mean square loss. Default is false.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the root square loss should be computed; otherwise, <c>false</c>.
    /// </value>
    public bool Root
    {
        get => root;
        set => root = value;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the
    ///     mean square loss should be computed. If <see cref="Root" />
    ///     is also set, computes the root mean square loss. Default is true.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the mean square loss should be computed; otherwise, <c>false</c>.
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
            error += Distance.SquareEuclidean(actual[i], Expected[i]);

        if (root)
            error = Math.Sqrt(error);

        if (mean)
            error = error / Expected.Length;

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
        {
            var u = actual[i] - Expected[i][0];
            error += u * u;
        }

        if (root)
            error = Math.Sqrt(error);

        if (mean)
            error = error / Expected.Length;

        return error;
    }
}