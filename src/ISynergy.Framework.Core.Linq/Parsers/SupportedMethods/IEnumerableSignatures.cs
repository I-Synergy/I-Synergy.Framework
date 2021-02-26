namespace ISynergy.Framework.Core.Linq.Parsers.SupportedMethods
{
    /// <summary>
    /// Interface IEnumerableSignatures
    /// </summary>
    internal interface IEnumerableSignatures : IBaseSignature
    {
        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Contains(object selector);
        /// <summary>
        /// Converts to array.
        /// </summary>
        void ToArray();
        /// <summary>
        /// Converts to list.
        /// </summary>
        void ToList();
    }
}
