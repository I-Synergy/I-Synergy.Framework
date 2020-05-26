using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    /// <summary>
    /// Interface ISubtractSignatures
    /// Implements the <see cref="IAddSignatures" />
    /// </summary>
    /// <seealso cref="IAddSignatures" />
    internal interface ISubtractSignatures : IAddSignatures
    {
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
        void F(DateTime? x, DateTime? y);
    }
}
