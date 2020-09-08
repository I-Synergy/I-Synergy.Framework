namespace ISynergy.Framework.Core.Linq.Abstractions
{
    /// <summary>
    /// Interface IKeywordsHelper
    /// </summary>
    public interface IKeywordsHelper
    {
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TryGetValue(string name, out object type);
    }
}
