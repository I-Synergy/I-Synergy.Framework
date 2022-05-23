using System;

namespace ISynergy.Framework.Mathematics.Optimization.Losses
{
    /// <summary>
    ///     Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// <typeparam name="TInput">The type for the expected data.</typeparam>
    /// <typeparam name="TScore">The type for the predicted score values.</typeparam>
    /// <typeparam name="TLoss">The type for the loss value. Default is double.</typeparam>
    [Serializable]
    public abstract class LossBase<TInput, TScore, TLoss> : ILoss<TScore, TLoss>
    {
        private TInput expected;

        /// <summary>
        ///     Gets the expected outputs (the ground truth).
        /// </summary>
        public TInput Expected
        {
            get => expected;
            protected set => expected = value;
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
        public abstract TLoss Loss(TScore actual);
    }

    /// <summary>
    ///     Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// <typeparam name="T">The type for the expected data.</typeparam>
    [Serializable]
    public abstract class LossBase<T> : LossBase<T, T, double>, ILoss<T>
    {
    }
}