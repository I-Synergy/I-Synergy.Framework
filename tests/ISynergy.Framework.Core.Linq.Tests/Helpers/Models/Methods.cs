namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class Methods.
    /// </summary>
    public class Methods
    {
        /// <summary>
        /// Method1s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Method1(int value)
        {
            return value == 1;
        }

        /// <summary>
        /// Method2s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Method2(object value)
        {
            return value != null && (int)value == 1;
        }

        /// <summary>
        /// Method3s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Method3(object value)
        {
            return value is Item item && item.Value == 1;
        }

        /// <summary>
        /// Class Item.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public int Value { get; set; }
        }
    }
}
