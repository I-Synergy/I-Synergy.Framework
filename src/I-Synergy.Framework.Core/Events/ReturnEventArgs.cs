using System;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class ReturnEventArgs.
    /// Implements the <see cref="EventArgs" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EventArgs" />
    public class ReturnEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnEventArgs{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ReturnEventArgs(T value)
        {
            Value = value;
        }
    }
}
