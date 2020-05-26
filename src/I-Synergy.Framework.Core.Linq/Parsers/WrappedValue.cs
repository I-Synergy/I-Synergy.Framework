namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class WrappedValue.
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    internal class WrappedValue<TValue>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedValue{TValue}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public WrappedValue(TValue value)
        {
            Value = value;
        }
    }
}
