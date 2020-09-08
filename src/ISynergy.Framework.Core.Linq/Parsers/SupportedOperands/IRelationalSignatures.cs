using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    /// <summary>
    /// Interface IRelationalSignatures
    /// Implements the <see cref="IArithmeticSignatures" />
    /// </summary>
    /// <seealso cref="IArithmeticSignatures" />
    internal interface IRelationalSignatures : IArithmeticSignatures
    {
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(string x, string y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(char x, char y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTime x, DateTime y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTimeOffset x, DateTimeOffset y);
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
        void F(char? x, char? y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTime? x, DateTime? y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(DateTimeOffset? x, DateTimeOffset? y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(TimeSpan? x, TimeSpan? y);
    }
}
