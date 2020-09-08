namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    /// <summary>
    /// Interface ILogicalSignatures
    /// </summary>
    internal interface ILogicalSignatures
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
    }
}
