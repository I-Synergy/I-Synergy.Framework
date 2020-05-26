using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    /// <summary>
    /// Interface IEqualitySignatures
    /// Implements the <see cref="IRelationalSignatures" />
    /// </summary>
    /// <seealso cref="IRelationalSignatures" />
    internal interface IEqualitySignatures : IRelationalSignatures
    {
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">if set to <c>true</c> [y].</param>
        void F(bool x, bool y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">if set to <c>true</c> [y].</param>
        void F(bool? x, bool? y);

        // Disabled 4 lines below because of : https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
        //void F(DateTime x, string y);
        //void F(DateTime? x, string y);
        //void F(string x, DateTime y);
        //void F(string x, DateTime? y);

        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(Guid x, Guid y);
        /// <summary>
        /// fs the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void F(Guid? x, Guid? y);

        // Disabled 4 lines below because of : https://github.com/StefH/System.Linq.Dynamic.Core/pull/200
        //void F(Guid x, string y);
        //void F(Guid? x, string y);
        //void F(string x, Guid y);
        //void F(string x, Guid? y);
    }
}
