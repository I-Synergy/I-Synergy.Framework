using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem
{
    /// <summary>
    /// A collection implementing this interface allows a more efficient collection of child entries
    /// </summary>
    public interface IRecusiveChildrenCollector
    {
        /// <summary>
        /// Gets all child entries up to the depth of <paramref name="maxDepth"/>
        /// </summary>
        /// <param name="maxDepth">The maximum depth (<see cref="int.MaxValue"/> as infinity)</param>
        /// <returns>All found child entries</returns>
        IAsyncEnumerable<IEntry> GetEntries(int maxDepth);
    }
}
