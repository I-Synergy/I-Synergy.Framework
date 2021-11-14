using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Tests.Fixtures
{
    /// <summary>
    /// Class ModelFixture.
    /// Implements the <see cref="ModelBase" />
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ModelBase" />
    /// <seealso cref="IDisposable" />
    public class ModelFixture<T> : ModelBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
        /// </summary>
        public ModelFixture()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public ModelFixture(T initialValue)
            : this()
        {
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the Value property value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (Value is null)
            {
                return string.Empty;
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
