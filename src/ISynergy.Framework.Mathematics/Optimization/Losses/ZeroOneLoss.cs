using ISynergy.Framework.Mathematics.Statistics;
using System;

namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     Accuracy loss, also known as zero-one-loss. This class
    ///     provides exactly the same functionality as <see cref="ZeroOneLoss" />
    ///     but has a more intuitive name. Both classes are interchangeable.
    /// </summary>
    [Serializable]
    public class AccuracyLoss : ZeroOneLoss
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AccuracyLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public AccuracyLoss(double[][] expected)
            : base(expected)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccuracyLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public AccuracyLoss(double[] expected)
            : base(expected)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccuracyLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public AccuracyLoss(int[] expected)
            : base(expected)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public AccuracyLoss(bool[] expected)
            : base(expected)
        {
        }
    }

    /// <summary>
    ///     Accuracy loss, also known as zero-one-loss.
    /// </summary>
    [Serializable]
    public class ZeroOneLoss : LossBase<int[]>, ILoss<bool[]>,
        ILoss<double[][]>, ILoss<double[]>
    {
        private bool mean = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(double[][] expected)
            : this(expected.ArgMax(0))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="classes">The number of classes.</param>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(int classes, double[][] expected)
            : this(classes, expected.ArgMax(0))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(double[] expected)
            : this(expected.ToZeroOne())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="classes">The number of classes.</param>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(int classes, double[] expected)
            : this(classes, expected.ToZeroOne())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(int[] expected)
            : this(expected.Max() + 1, expected)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="classes">The number of classes.</param>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(int classes, int[] expected)
        {
            NumberOfClasses = classes;
            Expected = NumberOfClasses == 2 && expected.IsMinusOnePlusOne() ? expected.ToZeroOne() : expected;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZeroOneLoss" /> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(bool[] expected)
            : this(2, expected.ToInt32())
        {
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the average
        ///     accuracy loss should be computed. Default is true.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the average accuracy loss should be computed; otherwise, <c>false</c>.
        /// </value>
        public bool Mean
        {
            get => mean;
            set => mean = value;
        }

        /// <summary>
        ///     Gets or sets the number of classes.
        /// </summary>
        /// <value>The number of classes.</value>
        public int NumberOfClasses { get; set; }

        /// <summary>
        ///     This flag indicates whether the expected class labels are binary.
        /// </summary>
        public bool IsBinary => NumberOfClasses == 2;

        /// <summary>
        ///     Computes the loss between the expected values (ground truth)
        ///     and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        ///     The loss value between the expected values and
        ///     the actual predicted values.
        /// </returns>
        public double Loss(bool[] actual)
        {
            return Loss(actual.ToZeroOne());
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
            return Loss(actual.ArgMax(0));
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
            return Loss(Classes.Decide(actual));
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
        public override double Loss(int[] actual)
        {
            NumberOfClasses = Math.Max(NumberOfClasses, actual.Max() + 1);

            if (NumberOfClasses == 2) actual = actual.ToZeroOne();

            var error = 0;
            for (var i = 0; i < Expected.Length; i++)
                if (Expected[i] != actual[i])
                    error++;

            if (mean)
                return error / (double)Expected.Length;
            return error;
        }
    }
}