using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    /// <summary>
    /// Interface IAddSignatures
    /// Implements the <see cref="IArithmeticSignatures" />
    /// </summary>
    /// <seealso cref="IArithmeticSignatures" />
    internal interface IAddSignatures : IArithmeticSignatures
    {
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTime x, TimeSpan y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(TimeSpan x, TimeSpan y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTime? x, TimeSpan? y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(TimeSpan? x, TimeSpan? y);
    }
}
